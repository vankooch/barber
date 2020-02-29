namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MQTTnet.Server;

    public class MqttClientDisconnectedHandler : IMqttServerClientDisconnectedHandler
    {
        private readonly ILogger _logger;

        public MqttClientDisconnectedHandler(ILogger<MqttClientDisconnectedHandler> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleClientDisconnectedAsync(MqttServerClientDisconnectedEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
