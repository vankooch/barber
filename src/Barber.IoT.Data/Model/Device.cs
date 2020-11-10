namespace Barber.IoT.Data.Model
{
    using System.Collections.Generic;
    using Barber.IoT.Authentication.Models;

    public class Device : DeviceModel<string>
    {
        public Device()
            : base()
        {
        }

        public virtual ICollection<DeviceActivity> Activites { get; set; } = new HashSet<DeviceActivity>();
    }
}
