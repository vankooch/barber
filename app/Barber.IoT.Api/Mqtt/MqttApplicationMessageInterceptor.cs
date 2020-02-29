namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MQTTnet.Server;

    public class MqttApplicationMessageInterceptor : IMqttServerApplicationMessageInterceptor
    {
        private readonly ILogger _logger;

        public MqttApplicationMessageInterceptor(ILogger<MqttApplicationMessageInterceptor> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
        {
            return Task.CompletedTask;
        }
    }
}
