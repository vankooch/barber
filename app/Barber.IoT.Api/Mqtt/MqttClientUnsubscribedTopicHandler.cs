namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MQTTnet.Server;

    public class MqttClientUnsubscribedTopicHandler : IMqttServerClientUnsubscribedTopicHandler
    {
        private readonly ILogger _logger;

        public MqttClientUnsubscribedTopicHandler(ILogger<MqttClientUnsubscribedTopicHandler> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task HandleClientUnsubscribedTopicAsync(MqttServerClientUnsubscribedTopicEventArgs eventArgs)
        {
            return Task.CompletedTask;
        }
    }
}
