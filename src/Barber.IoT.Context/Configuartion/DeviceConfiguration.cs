namespace Barber.IoT.Context.Configuartion
{
    using Barber.IoT.Data.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DeviceConfiguration : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            if (builder == null)
            {
                return;
            }

            _ = builder.ToTable(BarberIoTContextConfiguartion.TABLE_IOT_DEVICE, BarberIoTContextConfiguartion.SCHEMA_IOT);

            _ = builder.HasKey(u => u.Id);
            _ = builder.HasIndex(u => u.NormalizedName).HasName("NameIndex");

            _ = builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

            _ = builder.Property(u => u.Name).HasMaxLength(512);
            _ = builder.Property(u => u.NormalizedName).HasMaxLength(512);
        }
    }
}
