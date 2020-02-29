namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MQTTnet.Server;

    public class MqttClientConnectedHandler : IMqttServerClientConnectedHandler
    {
        private readonly ILogger _logger;

        public MqttClientConnectedHandler(ILogger<MqttClientConnectedHandler> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task HandleClientConnectedAsync(MqttServerClientConnectedEventArgs eventArgs)
        {
            return;
        }
    }
}
