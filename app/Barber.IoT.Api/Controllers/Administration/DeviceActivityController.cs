namespace Barber.IoT.Api.Controllers.Administration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication;
    using Barber.IoT.Data.Model;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Device activity management controller
    /// </summary>
    [Route("api/administration/device/activity")]
    public class DeviceActivityController : ApiBaseController
    {
        private readonly IDeviceActivityManager<DeviceActivity> _deviceManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceActivityController(
            IDeviceActivityManager<DeviceActivity> deviceManager)
            => this._deviceManager = deviceManager;

        /// <summary>
        /// Get device activity history
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IReadOnlyList<DeviceActivity>>> Get(string id)
            => this.Ok(await this._deviceManager.FindByDeviceIdAsync(id));
    }
}
