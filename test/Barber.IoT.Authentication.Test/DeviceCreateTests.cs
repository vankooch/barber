using System;
using System.Threading.Tasks;
using Barber.IoT.Authentication.EntityFrameworkCore;
using Barber.IoT.Context;
using Barber.IoT.Data.Model;
using FluentAssertions;
using Xunit;

namespace Barber.IoT.Authentication.Test
{
    public class DeviceCreateTests : Base.TestCommon
    {
        private readonly DeviceManager<Device> _deviceManager;

        public DeviceCreateTests()
            : base()
        {
            var contextCreator = this.Container.GetInstance<IBarberIoTContextCreator>();

            var t = new DeviceStore<Device, BarberIoTContext, string>(contextCreator.CreateDbContext());
            var dv = new IDeviceValidator<Device>[] { new DeviceValidator<Device>() };

            this._deviceManager = new DeviceManager<Device>(t, null, null, null, dv, null);
        }

        [Fact]
        public async Task WithOutPassword()
        {
            var device = new Device()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Device-1",
            };

            var result = await this._deviceManager.CreateAsync(device);
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();

            var resultDevice = await this._deviceManager.FindByIdAsync(device.Id);
            resultDevice.Should().NotBeNull();
            resultDevice.NormalizedName.Should().Be("DEVICE-1");
            resultDevice.PasswordHash.Should().BeNull();
        }

        [Fact]
        public async Task WithPassword()
        {
            var device = new Device()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Device-2",
            };

            var result = await this._deviceManager.CreateAsync(device, "password");
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();

            var resultDevice = await this._deviceManager.FindByIdAsync(device.Id);
            resultDevice.Should().NotBeNull();
            resultDevice.NormalizedName.Should().Be("DEVICE-2");
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();
        }
    }
}
