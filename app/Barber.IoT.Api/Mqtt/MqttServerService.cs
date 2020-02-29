namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Collections.Generic;
    using System.Net.WebSockets;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Barber.IoT.Api.Configuration;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using MQTTnet;
    using MQTTnet.Adapter;
    using MQTTnet.AspNetCore;
    using MQTTnet.Client.Publishing;
    using MQTTnet.Implementations;
    using MQTTnet.Server;
    using MQTTnet.Server.Status;

    public class MqttServerService
    {
        private readonly ILogger<MqttServerService> _logger;

        private readonly MqttApplicationMessageInterceptor _mqttApplicationMessageInterceptor;
        private readonly MqttClientConnectedHandler _mqttClientConnectedHandler;
        private readonly MqttClientDisconnectedHandler _mqttClientDisconnectedHandler;
        private readonly MqttClientSubscribedTopicHandler _mqttClientSubscribedTopicHandler;
        private readonly MqttClientUnsubscribedTopicHandler _mqttClientUnsubscribedTopicHandler;
        private readonly MqttServerConnectionValidator _mqttConnectionValidator;
        private readonly IMqttServer _mqttServer;
        private readonly MqttServerStorage _mqttServerStorage;
        private readonly MqttSubscriptionInterceptor _mqttSubscriptionInterceptor;
        private readonly MqttSettingsModel _settings;
        private readonly MqttWebSocketServerAdapter _webSocketServerAdapter;

        public MqttServerService(
            MqttSettingsModel mqttSettings,
            CustomMqttFactory mqttFactory,
            MqttClientConnectedHandler mqttClientConnectedHandler,
            MqttClientDisconnectedHandler mqttClientDisconnectedHandler,
            MqttClientSubscribedTopicHandler mqttClientSubscribedTopicHandler,
            MqttClientUnsubscribedTopicHandler mqttClientUnsubscribedTopicHandler,
            MqttServerConnectionValidator mqttConnectionValidator,
            MqttSubscriptionInterceptor mqttSubscriptionInterceptor,
            MqttApplicationMessageInterceptor mqttApplicationMessageInterceptor,
            MqttServerStorage mqttServerStorage,
            ILogger<MqttServerService> logger)
        {
            this._settings = mqttSettings ?? throw new ArgumentNullException(nameof(mqttSettings));
            this._mqttClientConnectedHandler = mqttClientConnectedHandler ?? throw new ArgumentNullException(nameof(mqttClientConnectedHandler));
            this._mqttClientDisconnectedHandler = mqttClientDisconnectedHandler ?? throw new ArgumentNullException(nameof(mqttClientDisconnectedHandler));
            this._mqttClientSubscribedTopicHandler = mqttClientSubscribedTopicHandler ?? throw new ArgumentNullException(nameof(mqttClientSubscribedTopicHandler));
            this._mqttClientUnsubscribedTopicHandler = mqttClientUnsubscribedTopicHandler ?? throw new ArgumentNullException(nameof(mqttClientUnsubscribedTopicHandler));
            this._mqttConnectionValidator = mqttConnectionValidator ?? throw new ArgumentNullException(nameof(mqttConnectionValidator));
            this._mqttSubscriptionInterceptor = mqttSubscriptionInterceptor ?? throw new ArgumentNullException(nameof(mqttSubscriptionInterceptor));
            this._mqttApplicationMessageInterceptor = mqttApplicationMessageInterceptor ?? throw new ArgumentNullException(nameof(mqttApplicationMessageInterceptor));
            this._mqttServerStorage = mqttServerStorage ?? throw new ArgumentNullException(nameof(mqttServerStorage));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._webSocketServerAdapter = new MqttWebSocketServerAdapter(mqttFactory.Logger.CreateChildLogger());

            var adapters = new List<IMqttServerAdapter>
            {
                new MqttTcpServerAdapter(mqttFactory.Logger.CreateChildLogger())
                {
                    TreatSocketOpeningErrorAsWarning = true, // Opening other ports than for HTTP is not allows in Azure App Services.
                },
                this._webSocketServerAdapter,
            };

            this._mqttServer = mqttFactory.CreateMqttServer(adapters);
        }

        public Task ClearRetainedApplicationMessagesAsync()
        {
            return this._mqttServer.ClearRetainedApplicationMessagesAsync();
        }

        public void Configure()
        {
            this._mqttServerStorage.Configure();

            this._mqttServer.ClientConnectedHandler = this._mqttClientConnectedHandler;
            this._mqttServer.ClientDisconnectedHandler = this._mqttClientDisconnectedHandler;
            this._mqttServer.ClientSubscribedTopicHandler = this._mqttClientSubscribedTopicHandler;
            this._mqttServer.ClientUnsubscribedTopicHandler = this._mqttClientUnsubscribedTopicHandler;

            this._mqttServer.StartAsync(this.CreateMqttServerOptions()).GetAwaiter().GetResult();

            this._logger.LogInformation("MQTT server started.");
        }

        public Task<IList<IMqttClientStatus>> GetClientStatusAsync()
        {
            return this._mqttServer.GetClientStatusAsync();
        }

        public Task<IList<MqttApplicationMessage>> GetRetainedApplicationMessagesAsync()
        {
            return this._mqttServer.GetRetainedApplicationMessagesAsync();
        }

        public Task<IList<IMqttSessionStatus>> GetSessionStatusAsync()
        {
            return this._mqttServer.GetSessionStatusAsync();
        }

        public Task<MqttClientPublishResult> PublishAsync(MqttApplicationMessage applicationMessage)
        {
            _ = applicationMessage ?? throw new ArgumentNullException(nameof(applicationMessage));

            return this._mqttServer.PublishAsync(applicationMessage);
        }

        public Task RunWebSocketConnectionAsync(WebSocket webSocket, HttpContext httpContext)
        {
            return this._webSocketServerAdapter.RunWebSocketConnectionAsync(webSocket, httpContext);
        }

        private IMqttServerOptions CreateMqttServerOptions()
        {
            var options = new MqttServerOptionsBuilder()
                .WithMaxPendingMessagesPerClient(this._settings.MaxPendingMessagesPerClient)
                .WithDefaultCommunicationTimeout(TimeSpan.FromSeconds(this._settings.CommunicationTimeout))
                .WithConnectionValidator(this._mqttConnectionValidator)
                .WithApplicationMessageInterceptor(this._mqttApplicationMessageInterceptor)
                .WithSubscriptionInterceptor(this._mqttSubscriptionInterceptor)
                .WithStorage(this._mqttServerStorage);

            // Configure unencrypted connections
            if (this._settings.TcpEndPoint.Enabled)
            {
                options.WithDefaultEndpoint();

                if (this._settings.TcpEndPoint.TryReadIPv4(out var address4))
                {
                    options.WithDefaultEndpointBoundIPAddress(address4);
                }

                if (this._settings.TcpEndPoint.TryReadIPv6(out var address6))
                {
                    options.WithDefaultEndpointBoundIPV6Address(address6);
                }

                if (this._settings.TcpEndPoint.Port > 0)
                {
                    options.WithDefaultEndpointPort(this._settings.TcpEndPoint.Port);
                }
            }
            else
            {
                options.WithoutDefaultEndpoint();
            }

            // Configure encrypted connections
            if (this._settings.EncryptedTcpEndPoint != null && this._settings.EncryptedTcpEndPoint.Enabled)
            {
                options
                    .WithEncryptedEndpoint()
                    .WithEncryptionSslProtocol(SslProtocols.Tls12);

                if (!string.IsNullOrEmpty(this._settings.EncryptedTcpEndPoint?.Certificate?.Path))
                {
                    IMqttServerCertificateCredentials? certificateCredentials = null;

                    if (!string.IsNullOrEmpty(this._settings.EncryptedTcpEndPoint?.Certificate?.Password))
                    {
                        certificateCredentials = new MqttServerCertificateCredentials
                        {
                            Password = this._settings?.EncryptedTcpEndPoint?.Certificate?.Password,
                        };
                    }

                    options.WithEncryptionCertificate(this._settings?.EncryptedTcpEndPoint?.Certificate?.ReadCertificate(), certificateCredentials);
                }

                if (this._settings!.EncryptedTcpEndPoint!.TryReadIPv4(out var address4))
                {
                    options.WithEncryptedEndpointBoundIPAddress(address4);
                }

                if (this._settings.EncryptedTcpEndPoint.TryReadIPv6(out var address6))
                {
                    options.WithEncryptedEndpointBoundIPV6Address(address6);
                }

                if (this._settings.EncryptedTcpEndPoint.Port > 0)
                {
                    options.WithEncryptedEndpointPort(this._settings.EncryptedTcpEndPoint.Port);
                }
            }
            else
            {
                options.WithoutEncryptedEndpoint();
            }

            if (this._settings.ConnectionBacklog > 0)
            {
                options.WithConnectionBacklog(this._settings.ConnectionBacklog);
            }

            if (this._settings.EnablePersistentSessions)
            {
                options.WithPersistentSessions();
            }

            return options.Build();
        }
    }
}
