namespace Barber.IoT.Authentication.EntityFrameworkCore
{
    using System;
    using Barber.IoT.Authentication.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IDeviceDbContext<TUser, TKey, TActivity>
        where TUser : DeviceModel<TKey>
        where TActivity : DeviceActivityModel<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of Devices.
        /// </summary>
        DbSet<TUser> Devices { get; }

        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of Device Activities.
        /// </summary>
        DbSet<TActivity> DeviceActivities { get; }
    }
}
