namespace Barber.IoT.Authentication.EntityFrameworkCore
{
    using System;
    using Barber.IoT.Authentication.Models;
    using Microsoft.EntityFrameworkCore;

    public interface IDeviceDbContext<TUser, TKey>
        where TUser : DeviceModel<TKey>
        where TKey : IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the <see cref="DbSet{TEntity}"/> of Users.
        /// </summary>
        DbSet<TUser> Devices { get; }
    }
}
