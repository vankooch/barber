namespace Barber.IoT.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication.Options;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Options;

    public class DeviceManager<TUser> : IDeviceManager<TUser>, IDisposable
        where TUser : class
    {
        private bool _disposed;

        public DeviceManager(
            IDeviceStore<TUser> store,
            ILookupNormalizer keyNormalizer,
            IOptions<DeviceOptions> identityOptionsAccessor,
            IOptions<PasswordHasherOptions> passwordOptionsAccessor,
            IEnumerable<IDeviceValidator<TUser>> userValidators,
            IEnumerable<IDevicePasswordValidator<TUser>> passwordValidators)
        {
            this.Store = store ?? throw new ArgumentNullException(nameof(store));
            this.Options = identityOptionsAccessor?.Value ?? new DeviceOptions();
            this.PasswordHasher = new PasswordHasher<TUser>(passwordOptionsAccessor);
            this.KeyNormalizer = keyNormalizer ?? new UpperInvariantLookupNormalizer();

            if (userValidators != null)
            {
                foreach (var v in userValidators)
                {
                    this.UserValidators.Add(v);
                }
            }

            if (passwordValidators != null)
            {
                foreach (var v in passwordValidators)
                {
                    this.PasswordValidators.Add(v);
                }
            }
        }

        /// <inheritdoc />
        public ILookupNormalizer KeyNormalizer { get; set; }

        /// <inheritdoc />
        public DeviceOptions Options { get; set; }

        /// <inheritdoc />
        public IPasswordHasher<TUser> PasswordHasher { get; set; }

        /// <inheritdoc />
        public IList<IDevicePasswordValidator<TUser>> PasswordValidators { get; private set; } = new List<IDevicePasswordValidator<TUser>>();

        /// <summary>
        /// Gets a flag indicating whether the backing user store supports user lock-outs.
        /// </summary>
        /// <value>
        /// true if the backing user store supports user lock-outs, otherwise false.
        /// </value>
        public virtual bool SupportsUserLockout
        {
            get
            {
                this.ThrowIfDisposed();
                return this.Store is IDeviceLockoutStore<TUser>;
            }
        }

        /// <summary>
        /// Gets a flag indicating whether the backing user store supports user passwords.
        /// </summary>
        /// <value>
        /// true if the backing user store supports user lock-outs, otherwise false.
        /// </value>
        public virtual bool SupportsUserPassword
        {
            get
            {
                this.ThrowIfDisposed();
                return this.Store is IDevicePasswordStore<TUser>;
            }
        }

        /// <inheritdoc />
        public IList<IDeviceValidator<TUser>> UserValidators { get; private set; } = new List<IDeviceValidator<TUser>>();

        /// <summary>
        /// Gets or sets the persistence store the manager operates over.
        /// </summary>
        /// <value>The persistence store the manager operates over.</value>
        protected internal IDeviceStore<TUser> Store { get; set; }

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

        /// <inheritdoc />
        public virtual Task<TUser> FindByIdAsync(string userId)
        {
            this.ThrowIfDisposed();
            return this.Store.FindByIdAsync(userId, this.CancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<IReadOnlyList<TUser>> FindByNameAsync(string userName)
        {
            this.ThrowIfDisposed();
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            userName = this.NormalizeName(userName);

            return this.Store.FindByNameAsync(userName, this.CancellationToken);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<TUser>> GetRegisteredAsync(int take = 100, int skip = 0)
            => this.Store.GetRegisteredAsync(take, skip, this.CancellationToken);

        /// <inheritdoc />
        public virtual Task<string> GetUserIdAsync(TUser user)
        {
            this.ThrowIfDisposed();
            return this.Store.GetUserIdAsync(user, this.CancellationToken);
        }

        /// <inheritdoc />
        public virtual Task<string> GetUserNameAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return this.Store.GetUserNameAsync(user, this.CancellationToken);
        }

        #region CRUD

        /// <inheritdoc />
        public async Task<IdentityResult> CreateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            var result = await this.ValidateUserAsync(user).ConfigureAwait(true);
            if (!result.Succeeded)
            {
                return result;
            }

            if (this.Options.Lockout.AllowedForNewUsers && this.SupportsUserLockout)
            {
                await this.GetUserLockoutStore().SetLockoutEnabledAsync(user, true, this.CancellationToken).ConfigureAwait(true);
            }

            await this.UpdateNormalizedUserNameAsync(user).ConfigureAwait(true);

            return await this.Store.CreateAsync(user, this.CancellationToken).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public async Task<IdentityResult> CreateAsync(TUser user, string password)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (this.SupportsUserPassword)
            {
                var passwordStore = this.GetPasswordStore();
                var result = await this.UpdatePasswordHash(passwordStore, user, password).ConfigureAwait(true);
                if (!result.Succeeded)
                {
                    return result;
                }
            }

            return await this.CreateAsync(user).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public Task<IdentityResult> DeleteAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return this.Store.DeleteAsync(user, this.CancellationToken);
        }

        /// <inheritdoc />
        public Task<IdentityResult> UpdateAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return this.UpdateUserAsync(user);
        }

        #endregion CRUD

        #region Password

        /// <inheritdoc />
        public async Task<IdentityResult> AddPasswordAsync(TUser user, string password)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var passwordStore = this.GetPasswordStore();
            var hash = await passwordStore.GetPasswordHashAsync(user, this.CancellationToken).ConfigureAwait(true);
            if (hash != null)
            {
                ////Logger.LogWarning(1, "User {userId} already has a password.", await this.GetUserIdAsync(user));
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserAlreadyHasPassword",
                    Description = "UserAlreadyHasPassword",
                });
            }

            var result = await this.UpdatePasswordHash(passwordStore, user, password).ConfigureAwait(true);
            if (!result.Succeeded)
            {
                return result;
            }

            return await this.UpdateUserAsync(user).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public async Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var passwordStore = this.GetPasswordStore();
            if (await this.VerifyPasswordAsync(passwordStore, user, currentPassword).ConfigureAwait(true) != PasswordVerificationResult.Failed)
            {
                var result = await this.UpdatePasswordHash(passwordStore, user, newPassword).ConfigureAwait(true);
                if (!result.Succeeded)
                {
                    return result;
                }

                return await this.UpdateUserAsync(user).ConfigureAwait(true);
            }

            ////Logger.LogWarning(2, "Change password failed for user {userId}.", await this.GetUserIdAsync(user));
            return IdentityResult.Failed(new IdentityError
            {
                Code = "PasswordMismatch",
                Description = "PasswordMismatch",
            });
        }

        /// <inheritdoc />
        public async Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                return false;
            }

            var passwordStore = this.GetPasswordStore();
            var result = await this.VerifyPasswordAsync(passwordStore, user, password).ConfigureAwait(true);
            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                await this.UpdatePasswordHash(passwordStore, user, password, validatePassword: false).ConfigureAwait(true);
                await this.UpdateUserAsync(user).ConfigureAwait(true);
            }

            var success = result != PasswordVerificationResult.Failed;
            if (!success)
            {
                ////Logger.LogWarning(0, "Invalid password for user {userId}.", await this.GetUserIdAsync(user));
            }

            return success;
        }

        /// <inheritdoc />
        public Task<bool> HasPasswordAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var passwordStore = this.GetPasswordStore();
            return passwordStore.HasPasswordAsync(user, this.CancellationToken);
        }

        /// <inheritdoc />
        public async Task<IdentityResult> RemovePasswordAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var passwordStore = this.GetPasswordStore();
            await this.UpdatePasswordHash(passwordStore, user, null, validatePassword: false).ConfigureAwait(true);
            return await this.UpdateUserAsync(user).ConfigureAwait(true);
        }

        #endregion Password

        #region Lock Out

        /// <inheritdoc />
        public virtual async Task<IdentityResult> AccessFailedAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // If this puts the user over the threshold for lockout, lock them out and reset the access failed count
            var store = this.GetUserLockoutStore();
            var count = await store.IncrementAccessFailedCountAsync(user, this.CancellationToken).ConfigureAwait(true);
            if (count < this.Options.Lockout.MaxFailedAccessAttempts)
            {
                return await this.UpdateUserAsync(user).ConfigureAwait(true);
            }

            ////Logger.LogWarning(12, "User {userId} is locked out.", await this.GetUserIdAsync(user));
            await store.SetLockoutEndDateAsync(
                user,
                DateTimeOffset.UtcNow.Add(this.Options.Lockout.DefaultLockoutTimeSpan),
                this.CancellationToken).ConfigureAwait(true);
            await store.ResetAccessFailedCountAsync(user, this.CancellationToken).ConfigureAwait(true);

            return await this.UpdateUserAsync(user).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public virtual async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var store = this.GetUserLockoutStore();

            return await store.GetAccessFailedCountAsync(user, this.CancellationToken).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public virtual async Task<bool> IsLockedOutAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var store = this.GetUserLockoutStore();
            if (!await store.GetLockoutEnabledAsync(user, this.CancellationToken).ConfigureAwait(true))
            {
                return false;
            }

            var lockoutTime = await store.GetLockoutEndDateAsync(user, this.CancellationToken).ConfigureAwait(true);
            return lockoutTime >= DateTimeOffset.UtcNow;
        }

        /// <inheritdoc />
        public virtual async Task<IdentityResult> ResetAccessFailedCountAsync(TUser user)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (await this.GetAccessFailedCountAsync(user).ConfigureAwait(true) == 0)
            {
                return IdentityResult.Success;
            }

            var store = this.GetUserLockoutStore();
            await store.ResetAccessFailedCountAsync(user, this.CancellationToken).ConfigureAwait(true);

            return await this.UpdateUserAsync(user).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public virtual async Task<IdentityResult> SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var store = this.GetUserLockoutStore();
            await store.SetLockoutEnabledAsync(user, enabled, this.CancellationToken).ConfigureAwait(true);

            return await this.UpdateUserAsync(user).ConfigureAwait(true);
        }

        /// <inheritdoc />
        public virtual async Task<IdentityResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd)
        {
            this.ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var store = this.GetUserLockoutStore();
            if (!await store.GetLockoutEnabledAsync(user, this.CancellationToken).ConfigureAwait(true))
            {
                ////Logger.LogWarning(11, "Lockout for user {userId} failed because lockout is not enabled for this user.", await this.GetUserIdAsync(user));
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "UserLockoutNotEnabled",
                    Description = "UserLockoutNotEnabled",
                });
            }

            await store.SetLockoutEndDateAsync(user, lockoutEnd, this.CancellationToken).ConfigureAwait(true);
            return await this.UpdateUserAsync(user).ConfigureAwait(true);
        }

        #endregion Lock Out

        /// <inheritdoc />
        public virtual string NormalizeName(string name)
            => (this.KeyNormalizer == null) ? name : this.KeyNormalizer.NormalizeName(name);

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
        /// Updates the normalized user name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose user name should be normalized and updated.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected virtual async Task UpdateNormalizedUserNameAsync(TUser user)
        {
            var normalizedName = this.NormalizeName(await this.GetUserNameAsync(user).ConfigureAwait(true));

            await this.Store.SetNormalizedUserNameAsync(user, normalizedName, this.CancellationToken).ConfigureAwait(true);
        }

        /// <summary>
        /// Called to update the user after validating and updating the normalized email/user name.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Whether the operation was successful.</returns>
        protected virtual async Task<IdentityResult> UpdateUserAsync(TUser user)
        {
            var result = await this.ValidateUserAsync(user).ConfigureAwait(true);
            if (!result.Succeeded)
            {
                return result;
            }

            await this.UpdateNormalizedUserNameAsync(user).ConfigureAwait(true);

            return await this.Store.UpdateAsync(user, this.CancellationToken).ConfigureAwait(true);
        }

        /// <summary>
        /// Should return <see cref="IdentityResult.Success"/> if validation is successful. This is
        /// called before updating the password hash.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="IdentityResult"/> representing whether validation was successful.</returns>
        protected async Task<IdentityResult> ValidatePasswordAsync(TUser user, string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return IdentityResult.Failed();
            }

            var errors = new List<IdentityError>();
            var isValid = true;
            foreach (var v in this.PasswordValidators)
            {
                var result = await v.ValidateAsync(this, user, password).ConfigureAwait(true);
                if (!result.Succeeded)
                {
                    if (result.Errors.Any())
                    {
                        errors.AddRange(result.Errors);
                    }

                    isValid = false;
                }
            }

            if (!isValid)
            {
                ////Logger.LogWarning(14, "User {userId} password validation failed: {errors}.", await this.GetUserIdAsync(user), string.Join(";", errors.Select(e => e.Code)));
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Should return <see cref="IdentityResult.Success"/> if validation is successful. This is
        /// called before saving the user via Create or Update.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>A <see cref="IdentityResult"/> representing whether validation was successful.</returns>
        protected async Task<IdentityResult> ValidateUserAsync(TUser user)
        {
            var errors = new List<IdentityError>();
            foreach (var v in this.UserValidators)
            {
                var result = await v.ValidateAsync(this, user).ConfigureAwait(true);
                if (!result.Succeeded)
                {
                    errors.AddRange(result.Errors);
                }
            }

            if (errors.Count > 0)
            {
                ////Logger.LogWarning(13, "User {userId} validation failed: {errors}.", await this.GetUserIdAsync(user), string.Join(";", errors.Select(e => e.Code)));
                return IdentityResult.Failed(errors.ToArray());
            }

            return IdentityResult.Success;
        }

        /// <summary>
        /// Returns a <see cref="PasswordVerificationResult"/> indicating the result of a password hash comparison.
        /// </summary>
        /// <param name="store">The store containing a user's password.</param>
        /// <param name="user">The user whose password should be verified.</param>
        /// <param name="password">The password to verify.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="PasswordVerificationResult"/>
        /// of the operation.
        /// </returns>
        protected virtual async Task<PasswordVerificationResult> VerifyPasswordAsync(IDevicePasswordStore<TUser> store, TUser user, string password)
        {
            if (store == null)
            {
                return PasswordVerificationResult.Failed;
            }

            var hash = await store.GetPasswordHashAsync(user, this.CancellationToken).ConfigureAwait(true);
            if (hash == null)
            {
                return PasswordVerificationResult.Failed;
            }

            return this.PasswordHasher.VerifyHashedPassword(user, hash, password);
        }

        private IDevicePasswordStore<TUser> GetPasswordStore()
        {
            if (!(this.Store is IDevicePasswordStore<TUser> cast))
            {
                throw new NotSupportedException("StoreNotIUserPasswordStore");
            }

            return cast;
        }

        private IDeviceLockoutStore<TUser> GetUserLockoutStore()
        {
            if (!(this.Store is IDeviceLockoutStore<TUser> cast))
            {
                throw new NotSupportedException("StoreNotIUserLockoutStore");
            }

            return cast;
        }

        private async Task<IdentityResult> UpdatePasswordHash(
          IDevicePasswordStore<TUser> passwordStore,
          TUser user,
          string? newPassword,
          bool validatePassword = true)
        {
            if (validatePassword)
            {
                var validate = await this.ValidatePasswordAsync(user, newPassword).ConfigureAwait(true);
                if (!validate.Succeeded)
                {
                    return validate;
                }
            }

            var hash = newPassword != null ? this.PasswordHasher.HashPassword(user, newPassword) : null;
            await passwordStore.SetPasswordHashAsync(user, hash, this.CancellationToken).ConfigureAwait(true);

            return IdentityResult.Success;
        }
    }
}
