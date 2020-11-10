using System;
using System.Linq;
using System.Threading.Tasks;
using Barber.IoT.Authentication.EntityFrameworkCore;
using Barber.IoT.Context;
using Barber.IoT.Data.Model;
using FluentAssertions;
using Xunit;

namespace Barber.IoT.Authentication.Test
{
    public class DevicePasswordTests : Base.TestCommon
    {
        private readonly DeviceManager<Device> _deviceManager;

        public DevicePasswordTests()
            : base()
        {
            var contextCreator = this.Container.GetInstance<IBarberIoTContextCreator>();

            var t = new DeviceStore<Device, DeviceActivity, BarberIoTContext, string>(contextCreator.CreateDbContext());

            this._deviceManager = new DeviceManager<Device>(t, null, null, null, null, null);
        }

        [Fact]
        public async Task AddPassword_Success()
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
            resultDevice.PasswordHash.Should().BeNull();

            var resultPassword = await this._deviceManager.AddPasswordAsync(resultDevice, "password");
            resultPassword.Should().NotBeNull();
            resultPassword.Succeeded.Should().BeTrue();

            resultDevice = await this._deviceManager.FindByIdAsync(device.Id);
            resultDevice.Should().NotBeNull();
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ChangePassword_Failed()
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
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();

            var resultPassword = await this._deviceManager.ChangePasswordAsync(resultDevice, "wrong", "newPassword");
            resultPassword.Should().NotBeNull();
            resultPassword.Succeeded.Should().BeFalse();
            resultPassword.Errors.Should().NotBeEmpty();
            resultPassword.Errors.Should().HaveCount(1);

            var error1 = resultPassword.Errors.First();
            error1.Code.Should().Be("PasswordMismatch");
        }

        [Fact]
        public async Task ChangePassword_Success()
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
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();

            var resultPassword = await this._deviceManager.ChangePasswordAsync(resultDevice, "password", "newPassword");
            resultPassword.Should().NotBeNull();
            resultPassword.Succeeded.Should().BeTrue();
        }

        [Fact]
        public async Task CheckPassword_Failed()
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
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();

            var resultPassword = await this._deviceManager.CheckPasswordAsync(resultDevice, "wrong");
            resultPassword.Should().BeFalse();
        }

        [Fact]
        public async Task CheckPassword_Success()
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
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();

            var resultPassword = await this._deviceManager.CheckPasswordAsync(resultDevice, "password");
            resultPassword.Should().BeTrue();
        }

        [Fact]
        public async Task RemovePassword_Success()
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
            resultDevice.PasswordHash.Should().NotBeNullOrEmpty();

            var resultPassword = await this._deviceManager.RemovePasswordAsync(resultDevice);
            resultPassword.Should().NotBeNull();
            resultPassword.Succeeded.Should().BeTrue();

            resultDevice = await this._deviceManager.FindByIdAsync(device.Id);
            resultDevice.Should().NotBeNull();
            resultDevice.PasswordHash.Should().BeNull();
        }
    }
}
