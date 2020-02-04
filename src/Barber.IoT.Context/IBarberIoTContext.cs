namespace Barber.IoT.Context
{
    using Barber.IoT.Authentication.EntityFrameworkCore;
    using Barber.IoT.Data.Model;

    public interface IBarberIoTContext : IDeviceDbContext<Device, string>
    {
    }
}
