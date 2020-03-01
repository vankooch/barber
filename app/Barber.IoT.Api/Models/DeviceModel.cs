namespace Barber.IoT.Api.Models
{
    using System;
    using Barber.IoT.Data.Model;

    /// <summary>
    /// Device information model
    /// </summary>
    public class DeviceModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceModel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DeviceModel(Device device)
        {
            _ = device ?? throw new ArgumentNullException(nameof(device));

            this.ConcurrencyStamp = device.ConcurrencyStamp;
            this.Id = device.Id;
            this.LockoutEnabled = device.LockoutEnabled;
            this.LockoutEnd = device.LockoutEnd;
            this.Name = device.Name;
        }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; } = string.Empty;

        /// <summary>
        /// Gets or sets the primary key for this device.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a flag indicating if the device could be locked out.
        /// </summary>
        /// <value>True if the device could be locked out, otherwise false.</value>
        public virtual bool LockoutEnabled { get; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any device lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the device is not locked out.
        /// </remarks>
        public virtual DateTimeOffset? LockoutEnd { get; }

        /// <summary>
        /// Device Name
        /// </summary>
        public virtual string Name { get; set; } = string.Empty;
    }
}
