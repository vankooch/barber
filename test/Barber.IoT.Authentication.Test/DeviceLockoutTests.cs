using System;
using System.Threading.Tasks;
using Barber.IoT.Authentication.EntityFrameworkCore;
using Barber.IoT.Context;
using Barber.IoT.Data.Model;
using FluentAssertions;
using Xunit;

namespace Barber.IoT.Authentication.Test
{
    public class DeviceLockoutTests : Base.TestCommon
    {
        private readonly DeviceManager<Device> _deviceManager;

        public DeviceLockoutTests()
            : base()
        {
            var contextCreator = this.Container.GetInstance<IBarberIoTContextCreator>();

            var t = new DeviceStore<Device, DeviceActivity, BarberIoTContext, string>(contextCreator.CreateDbContext());

            this._deviceManager = new DeviceManager<Device>(t, null, null, null, null, null);
        }

        [Fact]
        public async Task SetLockout_Success()
        {
            var device = new Device()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Device-2",
            };

            var result = await this._deviceManager.CreateAsync(device);
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();

            var resultDevice = await this._deviceManager.FindByIdAsync(device.Id);
            resultDevice.Should().NotBeNull();

            var resultLockout = await this._deviceManager.IsLockedOutAsync(resultDevice);
            resultLockout.Should().BeFalse();

            var resultLockoutUpdate = await this._deviceManager.SetLockoutEnabledAsync(resultDevice, true);
            resultLockoutUpdate.Should().NotBeNull();
            resultLockoutUpdate.Succeeded.Should().BeTrue();

            resultLockoutUpdate = await this._deviceManager.SetLockoutEndDateAsync(resultDevice, DateTime.Now.AddDays(1));
            resultLockoutUpdate.Should().NotBeNull();
            resultLockoutUpdate.Succeeded.Should().BeTrue();

            resultLockout = await this._deviceManager.IsLockedOutAsync(resultDevice);
            resultLockout.Should().BeTrue();
        }

        [Fact]
        public async Task SetLockout_AfterFaildSignIn()
        {
            var device = new Device()
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Device-2",
            };

            var result = await this._deviceManager.CreateAsync(device);
            result.Should().NotBeNull();
            result.Succeeded.Should().BeTrue();

            var resultDevice = await this._deviceManager.FindByIdAsync(device.Id);
            resultDevice.Should().NotBeNull();

            for (var i = 0; i < 5; i++)
            {
                var resultPassword = await this._deviceManager.AccessFailedAsync(resultDevice);
                resultPassword.Should().NotBeNull();
                resultPassword.Succeeded.Should().BeTrue();
            }

            var resultLockoutUpdate = await this._deviceManager.GetAccessFailedCountAsync(resultDevice);
            resultLockoutUpdate.Should().Be(0);

            var resultLockout = await this._deviceManager.IsLockedOutAsync(resultDevice);
            resultLockout.Should().BeTrue();
        }
    }
}
