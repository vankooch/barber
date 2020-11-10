namespace Barber.IoT.Authentication.EntityFrameworkCore
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;

    public class DeviceStore<TUser, TActivity, TContext, TKey> :
        IDeviceStore<TUser>,
        IDevicePasswordStore<TUser>,
        IDeviceLockoutStore<TUser>,
        IDeviceActivityStore<TActivity>
        where TUser : DeviceModel<TKey>
        where TActivity : DeviceActivityModel<TKey>
        where TKey : IEquatable<TKey>
        where TContext : DbContext
    {
        private bool _disposed;

        public DeviceStore(TContext context)
            => this.Context = context ?? throw new ArgumentNullException(nameof(context));

        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        /// <summary>
        /// Gets the database context for this store.
        /// </summary>
        public virtual TContext Context { get; private set; }

        /// <summary>
        /// DbSet of device activities.
        /// </summary>
        protected DbSet<TActivity> ActivitiesSet => this.Context.Set<TActivity>();

        /// <summary>
        /// DbSet of devices.
        /// </summary>
        protected DbSet<TUser> DeviceSet => this.Context.Set<TUser>();

        /// <summary>
        /// Converts the provided <paramref name="id"/> to a strongly typed key object.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An instance of <typeparamref name="TKey"/> representing the provided <paramref name="id"/>.</returns>
        public virtual TKey ConvertIdFromString(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return default;
            }

            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        /// <summary>
        /// Converts the provided <paramref name="id"/> to its string representation.
        /// </summary>
        /// <param name="id">The id to convert.</param>
        /// <returns>An <see cref="string"/> representation of the provided <paramref name="id"/>.</returns>
        public virtual string ConvertIdToString(TKey id)
        {
            if (object.Equals(id, default))
            {
                return string.Empty;
            }

            return id.ToString();
        }

        #region Basic

        /// <inheritdoc />
        public async Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.Context.Add(user);
            await this.SaveChanges(cancellationToken).ConfigureAwait(true);
            return IdentityResult.Success;
        }

        /// <inheritdoc />
        public async Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.Context.Remove(user);
            try
            {
                await this.SaveChanges(cancellationToken).ConfigureAwait(true);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ConcurrencyFailure",
                    Description = "ConcurrencyFailure",
                });
            }

            return IdentityResult.Success;
        }

        /// <inheritdoc />
        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            var id = this.ConvertIdFromString(userId);

            return this.DeviceSet.FindAsync(new object[] { id }, cancellationToken).AsTask();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TUser>> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            return await this.DeviceSet.Where(u => u.NormalizedName == normalizedUserName).ToArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(this.ConvertIdToString(user.Id));
        }

        /// <inheritdoc />
        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.Name);
        }

        /// <inheritdoc />
        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.NormalizedName = normalizedName;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.Context.Attach(user);
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            this.Context.Update(user);
            try
            {
                await this.SaveChanges(cancellationToken).ConfigureAwait(false);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "ConcurrencyFailure",
                    Description = "ConcurrencyFailure",
                });
            }

            return IdentityResult.Success;
        }

        #endregion Basic

        #region Password

        /// <inheritdoc />
        public Task<string?> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.PasswordHash);
        }

        /// <inheritdoc />
        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(user.PasswordHash != null);
        }

        /// <inheritdoc />
        public Task SetPasswordHashAsync(TUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        #endregion Password

        #region Lockout

        /// <inheritdoc />
        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <inheritdoc />
        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnabled);
        }

        /// <inheritdoc />
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return Task.FromResult(user.LockoutEnd);
        }

        /// <inheritdoc />
        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        /// <inheritdoc />
        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnabled = enabled;
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            user.LockoutEnd = lockoutEnd;
            return Task.CompletedTask;
        }

        #endregion Lockout

        #region Activity

        /// <inheritdoc />
        public async Task<IdentityResult> CreateActivityAsync(TActivity user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            this.Context.Add(user);
            await this.SaveChanges(cancellationToken).ConfigureAwait(true);
            return IdentityResult.Success;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TActivity>> FindActivityByDeviceIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            return await this.ActivitiesSet.Where(u => u.DeviceId.Equals(userId)).ToArrayAsync(cancellationToken).ConfigureAwait(false);
        }

        #endregion Activity

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<TUser>> GetRegisteredAsync(int take = 100, int skip = 0, CancellationToken cancellationToken = default)
            => await this.DeviceSet
                .OrderBy(e => e.Id)
                .Skip(skip)
                .Take(take)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

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

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected Task SaveChanges(CancellationToken cancellationToken)
            => this.AutoSaveChanges ? this.Context.SaveChangesAsync(cancellationToken) : Task.CompletedTask;

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
    }
}
