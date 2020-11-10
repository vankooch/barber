namespace Barber.IoT.Context.Configuartion
{
    using Barber.IoT.Data.Model;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class DeviceActivityConfiguration : IEntityTypeConfiguration<DeviceActivity>
    {
        public void Configure(EntityTypeBuilder<DeviceActivity> builder)
        {
            if (builder == null)
            {
                return;
            }

            _ = builder.ToTable(BarberIoTContextConfiguartion.TABLE_IOT_DEVICE_ACTIVITY, BarberIoTContextConfiguartion.SCHEMA_IOT);

            _ = builder.HasKey(u => u.Id);
            _ = builder.HasIndex(u => u.DeviceId).HasName("DeviceIdIndex");
        }
    }
}
