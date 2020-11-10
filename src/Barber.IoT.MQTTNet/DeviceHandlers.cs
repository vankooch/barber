namespace Barber.IoT.MQTTNet
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication.EntityFrameworkCore;
    using Barber.IoT.Authentication.Models;
    using Barber.IoT.Authentication.Options;
    using Barber.IoT.Data.Enums;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using MQTTnet.Protocol;
    using MQTTnet.Server;

    public class DeviceHandlers<TUser, TActivity, TContext> :
        IDeviceHandlers<TUser>,
        IMqttServerConnectionValidator,
        IMqttServerClientConnectedHandler,
        IMqttServerClientDisconnectedHandler,
        IDisposable
        where TUser : DeviceModel<string>
        where TActivity : DeviceActivityModel<string>, new()
        where TContext : DbContext
    {
        private bool _disposed;

        public DeviceHandlers(
            TContext context,
            ILookupNormalizer keyNormalizer,
            IOptions<PasswordHasherOptions> passwordOptionsAccessor,
            IOptions<DeviceOptions> identityOptionsAccessor)
        {
            this.Options = identityOptionsAccessor?.Value ?? new DeviceOptions();
            this.PasswordHasher = new PasswordHasher<TUser>(passwordOptionsAccessor);
            this.KeyNormalizer = keyNormalizer ?? new UpperInvariantLookupNormalizer();
            this.Store = new DeviceStore<TUser, TActivity, TContext, string>(context);
        }

        /// <inheritdoc />
        public ILookupNormalizer KeyNormalizer { get; set; }

        /// <inheritdoc />
        public DeviceOptions Options { get; set; }

        /// <inheritdoc />
        public IPasswordHasher<TUser> PasswordHasher { get; set; }

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal DeviceStore<TUser, TActivity, TContext, string> Store { get; set; }

        /// <summary>
        /// The cancellation token used to cancel operations.
        /// </summary>
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        /// <summary>
        /// Releases all resources used by the user manager.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual Task<TUser> FindByDeviceId(string userId)
        {
            this.ThrowIfDisposed();
            return this.Store.FindByIdAsync(userId, this.CancellationToken);
        }

        #region MQTTNet

        public Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
            => Task.CompletedTask;

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
            => Task.CompletedTask;

        public async Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            _ = context ?? throw new ArgumentNullException(nameof(context));
            if (string.IsNullOrWhiteSpace(context.Username))
            {
                context.ReasonCode = MqttConnectReasonCode.Success;

                return;
            }

            var device = await this.FindByDeviceId(context.Username).ConfigureAwait(false);
            if (device == null)
            {
                context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;

                return;
            }

            async Task CreateActicity(MqttConnectReasonCode code)
            {
                context.ReasonCode = code;
                await this.Store.CreateActivityAsync(new TActivity()
                {
                    DeviceId = device.Id,
                    State = (int)DeviceActivityStateType.ServerConnect,
                    Code = (int)code,
                    Payload = context.ClientId,
                }, this.CancellationToken).ConfigureAwait(false);
            }

            if (device.LockoutEnabled)
            {
                await CreateActicity(MqttConnectReasonCode.Banned).ConfigureAwait(false);

                return;
            }

            if (device.PasswordHash == null)
            {
                await CreateActicity(MqttConnectReasonCode.Success).ConfigureAwait(false);

                return;
            }

            var resultPassword = this.VerifyPasswordAsync(device, context.Password);
            if (resultPassword == PasswordVerificationResult.SuccessRehashNeeded)
            {
                await this.UpdatePasswordHash(device, context.Password).ConfigureAwait(true);
                await this.Store.UpdateAsync(device, this.CancellationToken).ConfigureAwait(true);
            }

            if (resultPassword == PasswordVerificationResult.Success)
            {
                await CreateActicity(MqttConnectReasonCode.Success).ConfigureAwait(false);

                return;
            }

            var count = await this.Store.IncrementAccessFailedCountAsync(device, this.CancellationToken).ConfigureAwait(true);
            if (count < this.Options.Lockout.MaxFailedAccessAttempts)
            {
                await this.Store.UpdateAsync(device, this.CancellationToken).ConfigureAwait(true);

                return;
            }

            await this.Store.SetLockoutEndDateAsync(
                device,
                DateTimeOffset.UtcNow.Add(this.Options.Lockout.DefaultLockoutTimeSpan),
                this.CancellationToken).ConfigureAwait(true);
            await this.Store.ResetAccessFailedCountAsync(device, this.CancellationToken).ConfigureAwait(true);
            await this.Store.UpdateAsync(device, this.CancellationToken).ConfigureAwait(true);

            context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
            await CreateActicity(MqttConnectReasonCode.BadUserNameOrPassword).ConfigureAwait(false);

            return;
        }

        #endregion MQTTNet

        /// <summary>
        /// Releases the unmanaged resources used by the role manager and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !this._disposed)
            {
                this._disposed = true;
            }
        }

        /// <summary>
        /// Throws if this class has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (this._disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        /// <summary>
        /// Returns a <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="user">The user whose password should be verified.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="PasswordVerificationResult"/>
        /// of the operation.
        /// </returns>
        protected virtual PasswordVerificationResult VerifyPasswordAsync(TUser user, string password)
        {
            if (user == null || user.PasswordHash == null)
            {
                return PasswordVerificationResult.Failed;
            }

            return this.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        }

        private async Task<IdentityResult> UpdatePasswordHash(
           TUser user,
           string? newPassword)
        {
            var hash = newPassword != null ? this.PasswordHasher.HashPassword(user, newPassword) : null;
            await this.Store.SetPasswordHashAsync(user, hash, this.CancellationToken).ConfigureAwait(true);

            return IdentityResult.Success;
        }
    }
}
