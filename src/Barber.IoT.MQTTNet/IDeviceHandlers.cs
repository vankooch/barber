namespace Barber.IoT.MQTTNet
{
    using System.Threading.Tasks;
    using Barber.IoT.Authentication.Models;
    using Barber.IoT.Authentication.Options;
    using Microsoft.AspNetCore.Identity;
    using MQTTnet.Server;

    public interface IDeviceHandlers<TUser> : IMqttServerConnectionValidator,
        IMqttServerClientConnectedHandler,
        IMqttServerClientDisconnectedHandler
        where TUser : DeviceModel<string>
    {
        /// <summary>
        /// The <see cref="ILookupNormalizer"/> used to normalize things like user and role names.
        /// </summary>
        ILookupNormalizer KeyNormalizer { get; set; }

        /// <summary>
        /// The <see cref="DeviceOptions"/> used to configure Identity.
        /// </summary>
        DeviceOptions Options { get; set; }

        /// <summary>
        /// The <see cref="IPasswordHasher{TUser}"/> used to hash passwords.
        /// </summary>
        IPasswordHasher<TUser> PasswordHasher { get; set; }

        /// <summary>
        /// Get device by Id, this should be the username in MQTTnet
        /// </summary>
        /// <param name="deviceId">Device id</param>
        /// <returns></returns>
        Task<TUser> FindByDeviceId(string deviceId);
    }
}
