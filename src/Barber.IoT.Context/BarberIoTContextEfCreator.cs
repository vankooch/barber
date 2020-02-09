namespace Barber.IoT.Context
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    public class BarberIoTContextEfCreator : IBarberIoTContextCreator, IDesignTimeDbContextFactory<BarberIoTContext>
    {
        private readonly IConfigurationRoot _configuartion;

        public BarberIoTContextEfCreator()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            this._configuartion = builder.Build();
        }

        public BarberIoTContext CreateDbContext()
        {
            var optionsBuilder = this.Options(new DbContextOptionsBuilder<BarberIoTContext>());

            return new BarberIoTContext(optionsBuilder.Options);
        }

        public BarberIoTContext CreateDbContext(string[] args)
        {
            var optionsBuilder = this.Options(new DbContextOptionsBuilder<BarberIoTContext>());

            return new BarberIoTContext(optionsBuilder.Options);
        }

        public DbContextOptionsBuilder<BarberIoTContext> Options(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(this._configuartion.GetConnectionString("barber-main"));
            if (optionsBuilder is DbContextOptionsBuilder<BarberIoTContext> match)
            {
                return match;
            }

            return new DbContextOptionsBuilder<BarberIoTContext>();
        }
    }
}
