// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Barber.IoT.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public class DevicePasswordValidator<TUser> : IDevicePasswordValidator<TUser>
        where TUser : class
    {
        public DevicePasswordValidator()
        {
        }

        /// <summary>
        /// Gets the <see cref="IdentityErrorDescriber"/> used to supply error text.
        /// </summary>
        /// <value>The <see cref="IdentityErrorDescriber"/> used to supply error text.</value>
        public IdentityErrorDescriber Describer { get; private set; } = new IdentityErrorDescriber();

        /// <summary>
        /// Returns a flag indicating whether the supplied character is a digit.
        /// </summary>
        /// <param name="c">The character to check if it is a digit.</param>
        /// <returns>True if the character is a digit, otherwise false.</returns>
        public virtual bool IsDigit(char c) => c >= '0' && c <= '9';

        /// <summary>
        /// Returns a flag indicating whether the supplied character is an ASCII letter or digit.
        /// </summary>
        /// <param name="c">The character to check if it is an ASCII letter or digit.</param>
        /// <returns>True if the character is an ASCII letter or digit, otherwise false.</returns>
        public virtual bool IsLetterOrDigit(char c) => this.IsUpper(c) || this.IsLower(c) || this.IsDigit(c);

        /// <summary>
        /// Returns a flag indicating whether the supplied character is a lower case ASCII letter.
        /// </summary>
        /// <param name="c">The character to check if it is a lower case ASCII letter.</param>
        /// <returns>True if the character is a lower case ASCII letter, otherwise false.</returns>
        public virtual bool IsLower(char c) => c >= 'a' && c <= 'z';

        /// <summary>
        /// Returns a flag indicating whether the supplied character is an upper case ASCII letter.
        /// </summary>
        /// <param name="c">The character to check if it is an upper case ASCII letter.</param>
        /// <returns>True if the character is an upper case ASCII letter, otherwise false.</returns>
        public virtual bool IsUpper(char c) => c >= 'A' && c <= 'Z';

        public Task<IdentityResult> ValidateAsync(IDeviceManager<TUser> manager, TUser user, string? password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            var errors = new List<IdentityError>();
            var options = manager.Options.Password;
            if (string.IsNullOrWhiteSpace(password) || password.Length < options.RequiredLength)
            {
                errors.Add(this.Describer.PasswordTooShort(options.RequiredLength));
            }

            if (options.RequireNonAlphanumeric && password.All(this.IsLetterOrDigit))
            {
                errors.Add(this.Describer.PasswordRequiresNonAlphanumeric());
            }

            if (options.RequireDigit && !password.Any(this.IsDigit))
            {
                errors.Add(this.Describer.PasswordRequiresDigit());
            }

            if (options.RequireLowercase && !password.Any(this.IsLower))
            {
                errors.Add(this.Describer.PasswordRequiresLower());
            }

            if (options.RequireUppercase && !password.Any(this.IsUpper))
            {
                errors.Add(this.Describer.PasswordRequiresUpper());
            }

            if (options.RequiredUniqueChars >= 1 && password.Distinct().Count() < options.RequiredUniqueChars)
            {
                errors.Add(this.Describer.PasswordRequiresUniqueChars(options.RequiredUniqueChars));
            }

            return Task.FromResult(!errors.Any() ? IdentityResult.Success : IdentityResult.Failed());
        }
    }
}
