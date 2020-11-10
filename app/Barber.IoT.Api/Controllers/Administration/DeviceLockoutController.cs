namespace Barber.IoT.Api.Controllers.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication;
    using Barber.IoT.Authentication.Options;
    using Barber.IoT.Data.Model;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// Device lockout management controller
    /// </summary>
    [Route("api/administration/device/lockout")]
    public class DeviceLockoutController : ApiBaseController
    {
        private readonly IDeviceManager<Device> _deviceManager;
        private readonly DeviceOptions _options;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceLockoutController(
            IDeviceManager<Device> deviceManager,
            IOptions<DeviceOptions> option)
        {
            this._deviceManager = deviceManager;
            this._options = option.Value;
        }

        /// <summary>
        /// Get device lockout state
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<bool>> GetState(string id)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            return this.Ok(await this._deviceManager.IsLockedOutAsync(user));
        }

        /// <summary>
        /// Ban given user for 99 Years
        /// </summary>
        /// <param name="id">Device id</param>
        /// <param name="state">Lockout state</param>
        /// <returns></returns>
        [HttpPut("ban/{id}")]
        public async Task<ActionResult<IdentityResult>> SetBan(string id, [FromBody] bool state)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            var lockoutDate = await this._deviceManager.SetLockoutEndDateAsync(user, state ? DateTime.Now.AddYears(99) : (DateTime?)null);
            if (!lockoutDate.Succeeded)
            {
                return this.Ok(lockoutDate);
            }

            return this.Ok(await this._deviceManager.SetLockoutEnabledAsync(user, state));
        }

        /// <summary>
        /// Change lockout state for a given device
        /// </summary>
        /// <param name="id">Device id</param>
        /// <param name="state">Lockout state</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<IdentityResult>> SetState(string id, [FromBody] bool state)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            var lockoutDate = await this._deviceManager.SetLockoutEndDateAsync(user, state ? DateTime.Now.Add(this._options.Lockout.DefaultLockoutTimeSpan) : (DateTime?)null);
            if (!lockoutDate.Succeeded)
            {
                return this.Ok(lockoutDate);
            }

            return this.Ok(await this._deviceManager.SetLockoutEnabledAsync(user, state));
        }
    }
}
