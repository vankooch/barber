namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Threading.Tasks;
    using Barber.IoT.Authentication;
    using Barber.IoT.Data.Model;
    using Microsoft.Extensions.Logging;
    using MQTTnet.Protocol;
    using MQTTnet.Server;

    public class MqttServerConnectionValidator : IMqttServerConnectionValidator
    {
        public const string WRAPPED_SESSION_ITEMS_KEY = "WRAPPED_ITEMS";
        private readonly IDeviceManager<Device> _deviceManager;
        private readonly ILogger _logger;

        public MqttServerConnectionValidator(ILogger<MqttServerConnectionValidator> logger)
        {
            ////this._deviceManager = deviceManager;
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ValidateConnectionAsync(MqttConnectionValidatorContext context)
        {
            ////if (string.IsNullOrWhiteSpace(context.Username))
            ////{
            ////    context.ReasonCode = MqttConnectReasonCode.Success;

            ////    return;
            ////}

            ////var device = await this._deviceManager.FindByIdAsync(context.Username);
            ////if (device == null)
            ////{
            ////    context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;

            ////    return;
            ////}

            ////if (await this._deviceManager.IsLockedOutAsync(device))
            ////{
            ////    context.ReasonCode = MqttConnectReasonCode.Banned;

            ////    return;
            ////}

            ////if (!await this._deviceManager.HasPasswordAsync(device))
            ////{
            ////    context.ReasonCode = MqttConnectReasonCode.Success;

            ////    return;
            ////}

            ////if (await this._deviceManager.CheckPasswordAsync(device, context.Password))
            ////{
            ////    context.ReasonCode = MqttConnectReasonCode.Success;

            ////    return;
            ////}

            ////var result = await this._deviceManager.AccessFailedAsync(device);
            ////if (!result.Succeeded)
            ////{
            ////    context.ReasonCode = MqttConnectReasonCode.UnspecifiedError;

            ////    return;
            ////}

            ////context.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;

            return;
        }
    }
}
