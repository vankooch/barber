namespace Barber.IoT.Api.Bootstrap
{
    using System;
    using System.Linq;
    using System.Net;
    using Barber.IoT.Api.Models;
    using Barber.IoT.Api.Mqtt;
    using Barber.IoT.Authentication.Options;
    using Barber.IoT.Context;
    using Barber.IoT.Data.Model;
    using Barber.IoT.MQTTNet;
    using Barber.IoT.MQTTNet.Configuration;
    using Barber.IoT.MQTTNet.Logging;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MQTTnet.Diagnostics;
    using MQTTnet.Server;

    public static class MqttNetExtensions
    {
        public static IServiceCollection AddMqttNetServer(
            this IServiceCollection services,
            IConfiguration configuration,
            BarberIoTContext barberIoTContext,
            PasswordHasherOptions passwordHasherOptions,
            DeviceOptions deviceOptions)
        {
            var mqttSettings = new MqttSettingsModel();
            configuration.Bind("MQTT", mqttSettings);
            services.AddSingleton(mqttSettings);

            var deviceHanders = new DeviceHandlers<Device, DeviceActivity, BarberIoTContext>(
                barberIoTContext,
                new UpperInvariantLookupNormalizer(),
                new OptionModel<PasswordHasherOptions>(passwordHasherOptions),
                new OptionModel<DeviceOptions>(deviceOptions));

            services.AddSingleton<IDeviceHandlers<Device>>(deviceHanders);
            services.AddSingleton<IMqttNetLogger, MqttNetLoggerWrapper>();
            services.AddSingleton<IMqttServerStorage, MqttServerStorage>();

            services.AddSingleton<IMqttServerClientSubscribedTopicHandler, MqttClientSubscribedTopicHandler>();
            services.AddSingleton<IMqttServerClientUnsubscribedTopicHandler, MqttClientUnsubscribedTopicHandler>();
            services.AddSingleton<IMqttServerSubscriptionInterceptor, MqttSubscriptionInterceptor>();
            services.AddSingleton<IMqttServerApplicationMessageInterceptor, MqttApplicationMessageInterceptor>();
            services.AddSingleton<MqttService<Device>>();

            return services;
        }

        public static IApplicationBuilder UseMqttServer(this IApplicationBuilder application, MqttService<Device> mqttServerService)
        {
            mqttServerService.StartServer();

            return application;
        }

        public static IApplicationBuilder UseMqttNetWebSocketEndpoint(this IApplicationBuilder application, MqttService<Device> mqttServerService, MqttSettingsModel mqttSettings)
        {
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
