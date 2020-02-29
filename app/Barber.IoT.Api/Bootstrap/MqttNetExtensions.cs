namespace Barber.IoT.Api.Bootstrap
{
    using System;
    using System.Linq;
    using System.Net;
    using Barber.IoT.Api.Configuration;
    using Barber.IoT.Api.Logging;
    using Barber.IoT.Api.Mqtt;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class MqttNetExtensions
    {
        public static IServiceCollection AddMqttNetServer(this IServiceCollection services, IConfiguration configuration)
        {
            var mqttSettings = new MqttSettingsModel();
            configuration.Bind("MQTT", mqttSettings);
            services.AddSingleton(mqttSettings);

            services.AddSingleton<CustomMqttFactory>();
            services.AddSingleton<MqttServerService>();
            services.AddSingleton<MqttNetLoggerWrapper>();
            services.AddSingleton<MqttServerStorage>();

            services.AddSingleton<MqttClientConnectedHandler>();
            services.AddSingleton<MqttClientDisconnectedHandler>();
            services.AddSingleton<MqttClientSubscribedTopicHandler>();
            services.AddSingleton<MqttClientUnsubscribedTopicHandler>();
            services.AddSingleton<MqttServerConnectionValidator>();
            services.AddSingleton<MqttSubscriptionInterceptor>();
            services.AddSingleton<MqttApplicationMessageInterceptor>();

            return services;
        }

        public static IApplicationBuilder UseMqttNetWebSocketEndpoint(this IApplicationBuilder application, MqttServerService mqttServerService, MqttSettingsModel mqttSettings)
        {
            mqttServerService.Configure();

            if (mqttSettings?.WebSocketEndPoint?.Enabled != true || string.IsNullOrEmpty(mqttSettings.WebSocketEndPoint.Path))
            {
                return application;
            }

            var webSocketOptions = new WebSocketOptions
            {
                KeepAliveInterval = TimeSpan.FromSeconds(mqttSettings.WebSocketEndPoint.KeepAliveInterval),
                ReceiveBufferSize = mqttSettings.WebSocketEndPoint.ReceiveBufferSize,
            };

            if (mqttSettings.WebSocketEndPoint.AllowedOrigins?.Any() == true)
            {
                foreach (var item in mqttSettings.WebSocketEndPoint.AllowedOrigins)
                {
                    webSocketOptions.AllowedOrigins.Add(item);
                }
            }

            application.UseWebSockets(webSocketOptions);

            application.Use(async (context, next) =>
            {
                if (context.Request.Path == mqttSettings.WebSocketEndPoint.Path)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);
                        await mqttServerService.RunWebSocketConnectionAsync(webSocket, context).ConfigureAwait(false);
                    }
                    else
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                else
                {
                    await next().ConfigureAwait(false);
                }
            });

            return application;
        }
    }
}
