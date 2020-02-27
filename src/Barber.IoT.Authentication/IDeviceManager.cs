namespace Barber.IoT.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication.Options;
    using Microsoft.AspNetCore.Identity;

    public interface IDeviceManager<TUser> : IDisposable
        where TUser : class
    {
        /// <summary>
        /// The <see cref="ILookupNormalizer"/> used to normalize things like user and role names.
        /// </summary>
        ILookupNormalizer KeyNormalizer { get; set; }

        /// <summary>
        /// The <see cref="DeviceOptions"/> used to configure Identity.
        /// </summary>
        DeviceOptions Options { get; set; }

        /// <summary>
        /// The <see cref="IPasswordHasher{TUser}"/> used to hash passwords.
        /// </summary>
        IPasswordHasher<TUser> PasswordHasher { get; set; }

        /// <summary>
        /// The <see cref="IPasswordValidator{TUser}"/> used to validate passwords.
        /// </summary>
        IList<IDevicePasswordValidator<TUser>> PasswordValidators { get; }

        /// <summary>
        /// The <see cref="IDeviceValidator{TUser}"/> used to validate users.
        /// </summary>
        IList<IDeviceValidator<TUser>> UserValidators { get; }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        Task<TUser> FindByIdAsync(string userId);

        /// <summary>
        /// Finds and returns a user, if any, who has the specified user name.
        /// </summary>
        /// <param name="userName">The user name to search for.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userName"/> if it exists.
        /// </returns>
        Task<TUser> FindByNameAsync(string userName);

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
        Task<string> GetUserIdAsync(TUser user);

        /// <summary>
        /// Gets the user name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
        Task<string> GetUserNameAsync(TUser user);

        #region CRUD

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the backing store with no password,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> CreateAsync(TUser user);

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the backing store with given password,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="password">The password for the user to hash and store.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> CreateAsync(TUser user, string password);

        /// <summary>
        /// Deletes the specified <paramref name="user"/> from the backing store.
        /// </summary>
        /// <param name="user">The user to delete.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> DeleteAsync(TUser user);

        /// <summary>
        /// Updates the specified <paramref name="user"/> in the backing store.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> UpdateAsync(TUser user);

        #endregion CRUD

        #region Password

        /// <summary>
        /// Adds the <paramref name="password"/> to the specified <paramref name="user"/> only if the user
        /// does not already have a password.
        /// </summary>
        /// <param name="user">The user whose password should be set.</param>
        /// <param name="password">The password to set.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> AddPasswordAsync(TUser user, string password);

        /// <summary>
        /// Changes a user's password after confirming the specified <paramref name="currentPassword"/> is correct,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose password should be set.</param>
        /// <param name="currentPassword">The current password to validate before changing.</param>
        /// <param name="newPassword">The new password to set for the specified <paramref name="user"/>.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> ChangePasswordAsync(TUser user, string currentPassword, string newPassword);

        /// <summary>
        /// Returns a flag indicating whether the given <paramref name="password"/> is valid for the
        /// specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose password should be validated.</param>
        /// <param name="password">The password to validate</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing true if
        /// the specified <paramref name="password" /> matches the one store for the <paramref name="user"/>,
        /// otherwise false.</returns>
        Task<bool> CheckPasswordAsync(TUser user, string password);

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/> has a password.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether they have a password or not.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a password
        /// otherwise false.
        /// </returns>
        Task<bool> HasPasswordAsync(TUser user);

        /// <summary>
        /// Removes a user's password.
        /// </summary>
        /// <param name="user">The user whose password should be removed.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        Task<IdentityResult> RemovePasswordAsync(TUser user);

        #endregion Password

        #region Lock Out

        /// <summary>
        /// Increments the access failed count for the user as an asynchronous operation.
        /// If the failed access account is greater than or equal to the configured maximum number of attempts,
        /// the user will be locked out for the configured lockout time span.
        /// </summary>
        /// <param name="user">The user whose failed access count to increment.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
        Task<IdentityResult> AccessFailedAsync(TUser user);

        /// <summary>
        /// Retrieves the current number of failed accesses for the given <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose access failed count should be retrieved for.</param>
        /// <returns>The <see cref="Task"/> that contains the result the asynchronous operation, the current failed access count
        /// for the user.</returns>
        Task<int> GetAccessFailedCountAsync(TUser user);

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user"/> his locked out,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose locked out status should be retrieved.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, true if the specified <paramref name="user "/>
        /// is locked out, otherwise false.
        /// </returns>
        Task<bool> IsLockedOutAsync(TUser user);

        /// <summary>
        /// Resets the access failed count for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
        Task<IdentityResult> ResetAccessFailedCountAsync(TUser user);

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user"/> is locked out,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose locked out status should be set.</param>
        /// <param name="enabled">Flag indicating whether the user is locked out or not.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, the <see cref="IdentityResult"/> of the operation
        /// </returns>
        Task<IdentityResult> SetLockoutEnabledAsync(TUser user, bool enabled);

        /// <summary>
        /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the operation.</returns>
        Task<IdentityResult> SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd);

        #endregion Lock Out
    }
}
