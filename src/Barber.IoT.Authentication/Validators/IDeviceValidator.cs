namespace Barber.IoT.Authentication
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public interface IDeviceValidator<TUser> where TUser : class
    {
        /// <summary>
        /// Validates the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
        /// <param name="user">The user to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the validation operation.</returns>
        Task<IdentityResult> ValidateAsync(IDeviceManager<TUser> manager, TUser user);
    }
}
