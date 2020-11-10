namespace Barber.IoT.Api.Controllers.Administration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication;
    using Barber.IoT.Data.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Device password management controller
    /// </summary>
    [Route("api/administration/device/password")]
    public class DevicePasswordController : ApiBaseController
    {
        private readonly IDeviceManager<Device> _deviceManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public DevicePasswordController(
            IDeviceManager<Device> deviceManager)
            => this._deviceManager = deviceManager;

        /// <summary>
        /// Delete password
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            return this.Ok(await this._deviceManager.RemovePasswordAsync(user));
        }

        /// <summary>
        /// Check if device has set a password
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> GetState(string id)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            return this.Ok(await this._deviceManager.HasPasswordAsync(user));
        }

        /// <summary>
        /// Ban given user for 99 Years
        /// </summary>
        /// <param name="id">Device id</param>
        /// <param name="password">New password</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IdentityResult>> SetPassword(string id, [FromBody] string password)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            if (await this._deviceManager.HasPasswordAsync(user))
            {
                return this.Ok(await this._deviceManager.ChangePasswordAsync(user, password));
            }

            return this.Ok(await this._deviceManager.AddPasswordAsync(user, password));
        }
    }
}
