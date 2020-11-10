namespace Barber.IoT.Authentication
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public interface IDeviceActivityStore<TActivity>
        where TActivity : class
    {
        /// <summary>
        /// Creates the specified activity for a given device
        /// </summary>
        /// <param name="user">The activity to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        Task<IdentityResult> CreateActivityAsync(TActivity user, CancellationToken cancellationToken);

        /// <summary>
        /// Finds and returns all activities for a given device id <paramref name="userId"/>.
        /// </summary>
        /// <param name="userId">The user ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        Task<IReadOnlyList<TActivity>> FindActivityByDeviceIdAsync(string userId, CancellationToken cancellationToken);
    }
}
