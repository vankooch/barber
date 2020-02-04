namespace Barber.IoT.Context
{
    using Microsoft.EntityFrameworkCore;

    public class BarberIoTContextDiCreator : IBarberIoTContextCreator
    {
        private readonly DbContextOptionsBuilder<BarberIoTContext> _dbContextOptionsBuilder;

        public BarberIoTContextDiCreator(DbContextOptionsBuilder<BarberIoTContext> configuration) => this._dbContextOptionsBuilder = configuration;

        public BarberIoTContext CreateDbContext()
            => new BarberIoTContext(this._dbContextOptionsBuilder.Options);

        public DbContextOptionsBuilder<BarberIoTContext> Options(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder as DbContextOptionsBuilder<BarberIoTContext>;
    }
}
