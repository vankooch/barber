#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

namespace Barber.IoT.Authentication.Models
{
    using System;

    public class DeviceModel<TKey> where TKey : IEquatable<TKey>
    {
        public DeviceModel()
        {
        }

        /// <summary>
        /// Gets or sets the number of failed login attempts for the current device.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// A random value that must change whenever a user is persisted to the store
        /// </summary>
        public virtual string ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the primary key for this device.
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if the device could be locked out.
        /// </summary>
        /// <value>True if the device could be locked out, otherwise false.</value>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// Gets or sets the date and time, in UTC, when any device lockout ends.
        /// </summary>
        /// <remarks>
        /// A value in the past means the device is not locked out.
        /// </remarks>
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        /// <summary>
        /// Device Name
        /// </summary>
        public virtual string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the normalized device name.
        /// </summary>
        public virtual string NormalizedName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a salted and hashed representation of the password for this device.
        /// </summary>
        public virtual string? PasswordHash { get; set; }

        /// <summary>
        /// A random value that must change whenever a devices credentials change (password changed, login removed)
        /// </summary>
        public virtual string? SecurityStamp { get; set; }

        /// <summary>
        /// Returns the name for this device.
        /// </summary>
        public override string ToString()
            => this.Name;
    }
}

#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
