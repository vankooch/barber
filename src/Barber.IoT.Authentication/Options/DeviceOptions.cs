namespace Barber.IoT.Authentication.Options
{
    using Microsoft.AspNetCore.Identity;

    public class DeviceOptions
    {
        public DeviceOptions()
        {
        }

        public LockoutOptions Lockout { get; set; } = new LockoutOptions();

        public PasswordOptions Password { get; set; } = new PasswordOptions();
    }
}
