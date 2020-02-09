#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace Barber.IoT.Context
{
    using System;
    using Barber.IoT.Data.Model;
    using Microsoft.EntityFrameworkCore;
    using Npgsql.NameTranslation;

    public class BarberIoTContext : DbContext, IBarberIoTContext
    {
        public BarberIoTContext()
            : base()
        {
        }

        public BarberIoTContext(DbContextOptions<BarberIoTContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Device> Devices { get; private set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                return;
            }

            base.OnModelCreating(modelBuilder);

            // Main
            _ = modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberIoTContext).Assembly);

            // Fix naming
            this.FixSnakeCaseNames(modelBuilder);
        }

        private void FixSnakeCaseNames(ModelBuilder modelBuilder)
        {
            var mapper = new NpgsqlSnakeCaseNameTranslator();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // modify column names
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(mapper.TranslateMemberName(property.GetColumnName()));
                }

                // modify table name
                entity.SetTableName(mapper.TranslateMemberName(entity.GetTableName()));
                entity.SetSchema(mapper.TranslateMemberName(entity.GetSchema()));

                // move asp_net tables into schema 'identity'
                if (entity.GetTableName().StartsWith("asp_net_", StringComparison.InvariantCulture))
                {
                    entity.SetTableName(entity.GetTableName().Replace("asp_net_", string.Empty));
                    entity.SetSchema("identity");
                }
            }
        }
    }
}
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
