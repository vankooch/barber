namespace Barber.IoT.Api.Bootstrap
{
    using System;
    using Barber.IoT.Authentication;
    using Barber.IoT.Authentication.EntityFrameworkCore;
    using Barber.IoT.Authentication.Models;
    using Barber.IoT.Authentication.Options;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class DeviceIdentityExtensions
    {
        public static IServiceCollection AddDeviceIdentityManager<TUser, TContext, TKey>(this IServiceCollection services)
            where TUser : DeviceModel<TKey>
            where TKey : IEquatable<TKey>
            where TContext : DbContext
        {
            _ = services.Configure<DeviceOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.AllowedForNewUsers = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });

            _ = services.Configure<PasswordHasherOptions>(options =>
            {
                options.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3;
            });

            services.TryAddScoped<IDeviceStore<TUser>, DeviceStore<TUser, TContext, TKey>>();
            services.TryAddScoped<IDeviceValidator<TUser>, DeviceValidator<TUser>>();
            services.TryAddScoped<IDevicePasswordValidator<TUser>, DevicePasswordValidator<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IDeviceManager<TUser>, DeviceManager<TUser>>();

            return services;
        }
    }
}
