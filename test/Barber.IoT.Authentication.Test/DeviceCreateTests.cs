using System;
using Barber.IoT.Authentication.EntityFrameworkCore;
using Barber.IoT.Context;
using Barber.IoT.Data.Model;
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

            this._deviceManager = new DeviceManager<Device>(t, null, null, null, null);
        }

        [Fact]
        public void WithOutPassword()
        {

        }
    }
}
