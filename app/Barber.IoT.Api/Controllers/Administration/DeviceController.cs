namespace Barber.IoT.Api.Controllers.Administration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Barber.IoT.Api.Models;
    using Barber.IoT.Authentication;
    using Barber.IoT.Data.Model;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Device management controller
    /// </summary>
    [Route("api/administration/device")]
    public class DeviceController : ApiBaseController
    {
        private readonly IDeviceManager<Device> _deviceManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceController(
            IDeviceManager<Device> deviceManager)
            => this._deviceManager = deviceManager;

        /// <summary>
        /// Delete a device from data store
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeviceModel>> Delete(string id)
        {
            var user = await this._deviceManager.FindByIdAsync(id) ?? throw new KeyNotFoundException();
            return this.Ok(await this._deviceManager.DeleteAsync(user));
        }

        /// <summary>
        /// Get stored device information, by given device id
        /// </summary>
        /// <param name="id">Device id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceModel>> GetById(string id)
            => this.Ok(new DeviceModel(await this._deviceManager.FindByIdAsync(id)));

        /// <summary>
        /// Get stored device information, by given device name
        /// </summary>
        /// <param name="name">Device name</param>
        /// <returns></returns>
        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<DeviceModel>>> GetByName(string name)
            => this.Ok((await this._deviceManager.FindByNameAsync(name))
                .Select(e => new DeviceModel(e)));

        /// <summary>
        /// List all stored devices. This method has pagination support.
        /// </summary>
        /// <param name="take">Devices per page, maximum is 1000</param>
        /// <param name="skip">Devices to skip</param>
        /// <returns></returns>
        [HttpGet("page/{take}/{skip}")]
        public async Task<ActionResult<IEnumerable<DeviceModel>>> GetPage(int take, int skip)
            => this.Ok((await this._deviceManager
                .GetRegisteredAsync(take > 1000 ? 1000 : take, skip))
                .Select(e => new DeviceModel(e)));

        /// <summary>
        /// Create a new device
        /// </summary>
        /// <param name="value">New device data</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<DeviceModel>> Post([FromBody] DeviceModel value)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var device = new Device()
            {
                Id = Guid.NewGuid().ToString(),
                Name = value.Name,
            };

            return this.Ok(await this._deviceManager.CreateAsync(device));
        }

        /// <summary>
        /// Update a given device
        /// </summary>
        /// <param name="id">Device id</param>
        /// <param name="value">Updated device data</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<DeviceModel>> Put(string id, [FromBody] DeviceModel value)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (id != value.Id)
            {
                return this.BadRequest();
            }

            var device = new Device()
            {
                Id = value.Id,
                Name = value.Name,
                ConcurrencyStamp = value.ConcurrencyStamp,
            };

            return this.Ok(await this._deviceManager.UpdateAsync(device));
        }
    }
}
