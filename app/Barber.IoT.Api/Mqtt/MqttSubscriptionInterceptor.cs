namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using MQTTnet.Server;

    public class MqttSubscriptionInterceptor : IMqttServerSubscriptionInterceptor
    {
        private readonly ILogger _logger;

        public MqttSubscriptionInterceptor(ILogger<MqttSubscriptionInterceptor> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task InterceptSubscriptionAsync(MqttSubscriptionInterceptorContext context)
        {
            return Task.CompletedTask;
        }
    }
}
