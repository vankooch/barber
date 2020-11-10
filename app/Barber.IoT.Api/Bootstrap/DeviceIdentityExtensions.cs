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
        public static (PasswordHasherOptions passwordOptions, DeviceOptions deviceOptions) AddDeviceIdentityManager<TUser, TActivity, TContext, TKey>(this IServiceCollection services)
            where TUser : DeviceModel<TKey>
            where TActivity : DeviceActivityModel<TKey>
            where TKey : IEquatable<TKey>
            where TContext : DbContext
        {
            // Password settings
            var deviceOptions = new DeviceOptions();
            deviceOptions.Password.RequireDigit = true;
            deviceOptions.Password.RequiredLength = 6;
            deviceOptions.Password.RequireNonAlphanumeric = false;
            deviceOptions.Password.RequireUppercase = false;
            deviceOptions.Password.RequireLowercase = false;

            // Lockout settings
            deviceOptions.Lockout.AllowedForNewUsers = false;
            deviceOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            deviceOptions.Lockout.MaxFailedAccessAttempts = 5;

            var passwordOptions = new PasswordHasherOptions
            {
                CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3,
            };

            _ = services.Configure<DeviceOptions>(options =>
            {
                options = deviceOptions;
            });

            _ = services.Configure<LockoutOptions>(options =>
            {
                options = deviceOptions.Lockout;
            });

            _ = services.Configure<PasswordHasherOptions>(options =>
            {
                options = passwordOptions;
            });

            services.TryAddScoped<IDeviceStore<TUser>, DeviceStore<TUser, TActivity, TContext, TKey>>();
            services.TryAddScoped<IDeviceActivityStore<TActivity>, DeviceStore<TUser, TActivity, TContext, TKey>>();
            services.TryAddScoped<IDeviceValidator<TUser>, DeviceValidator<TUser>>();
            services.TryAddScoped<IDevicePasswordValidator<TUser>, DevicePasswordValidator<TUser>>();
            services.TryAddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.TryAddScoped<IDeviceManager<TUser>, DeviceManager<TUser>>();
            services.TryAddScoped<IDeviceActivityManager<TActivity>, DeviceActivityManager<TActivity>>();

            return (passwordOptions, deviceOptions);
        }
    }
}
