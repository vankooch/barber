namespace Barber.IoT.Context
{
    using Microsoft.EntityFrameworkCore;

    public interface IBarberIoTContextCreator
    {
        BarberIoTContext CreateDbContext();

        DbContextOptionsBuilder<BarberIoTContext> Options(DbContextOptionsBuilder optionsBuilder);
    }
}
