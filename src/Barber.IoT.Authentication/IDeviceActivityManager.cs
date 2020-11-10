namespace Barber.IoT.Authentication
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public interface IDeviceActivityManager<TActivity>
        where TActivity : class
    {
        /// <summary>
        /// Creates the specified activity for a given device
        /// </summary>
        /// <param name="activity">The activity to create.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        Task<IdentityResult> CreateAsync(TActivity activity);

        /// <summary>
        /// Get last 1000 activities for given device
        /// </summary>
        /// <param name="deviceId">Device Id</param>
        /// <returns></returns>
        Task<IReadOnlyList<TActivity>> FindByDeviceIdAsync(string deviceId);
    }
}
