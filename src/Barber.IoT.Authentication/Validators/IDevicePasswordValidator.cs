namespace Barber.IoT.Authentication
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public interface IDevicePasswordValidator<TUser>
        where TUser : class
    {
        /// <summary>
        /// Validates a password as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> to retrieve the <paramref name="user"/> properties from.</param>
        /// <param name="user">The user whose password should be validated.</param>
        /// <param name="password">The password supplied for validation</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<IdentityResult> ValidateAsync(IDeviceManager<TUser> manager, TUser user, string? password);
    }
}
