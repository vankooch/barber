// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Barber.IoT.Authentication
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;

    public class DeviceValidator<TUser> : IDeviceValidator<TUser>
        where TUser : class
    {
        public DeviceValidator()
        {
        }

        /// <summary>
        /// Gets the <see cref="IdentityErrorDescriber"/> used to supply error text.
        /// </summary>
        /// <value>The <see cref="IdentityErrorDescriber"/> used to supply error text.</value>
        public IdentityErrorDescriber Describer { get; private set; } = new IdentityErrorDescriber();

        /// <summary>
        /// Validates the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="manager">The <see cref="UserManager{TUser}"/> that can be used to retrieve user properties.</param>
        /// <param name="user">The user to validate.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the validation operation.</returns>
        public async Task<IdentityResult> ValidateAsync(IDeviceManager<TUser> manager, TUser user)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var errors = new List<IdentityError>();
            await this.ValidateUserName(manager, user, errors).ConfigureAwait(true);

            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
        }

        private async Task ValidateUserName(IDeviceManager<TUser> manager, TUser user, ICollection<IdentityError> errors)
        {
            var userName = await manager.GetUserNameAsync(user).ConfigureAwait(true);
            if (string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(this.Describer.InvalidUserName(userName));
            }
            else
            {
                var owner = await manager.FindByNameAsync(userName).ConfigureAwait(true);
                if (owner != null
                    && !string.Equals(
                        await manager.GetUserIdAsync(owner).ConfigureAwait(true),
                        await manager.GetUserIdAsync(user).ConfigureAwait(true),
                        StringComparison.InvariantCulture))
                {
                    errors.Add(this.Describer.DuplicateUserName(userName));
                }
            }
        }
    }
}
