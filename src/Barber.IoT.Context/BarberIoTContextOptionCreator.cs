namespace Barber.IoT.Context
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;

    public static class BarberIoTContextOptionCreator
    {
        public static DbContextOptionsBuilder<BarberIoTContext> GetOptionsBuilder(IConfiguration configuration, string name)
        {
            var connectionString = configuration.GetConnectionString(name) ?? throw new KeyNotFoundException($"Could not find connection string with name: {name}");
            var contextBuilder = new DbContextOptionsBuilder<BarberIoTContext>();

            return contextBuilder.AddBarberOptions(connectionString);
        }

        public static DbContextOptionsBuilder<BarberIoTContext> AddBarberOptions(this DbContextOptionsBuilder contextBuilder, string connectionString)
        {
            if (connectionString.StartsWith("Filename=", System.StringComparison.InvariantCulture))
            {
                contextBuilder.UseSqlite(connectionString);
            }
            else
            {
                contextBuilder.UseNpgsql(connectionString);
            }

            return contextBuilder as DbContextOptionsBuilder<BarberIoTContext>;
        }
    }
}
