namespace Barber.IoT.Api.Mqtt
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Barber.IoT.MQTTNet.Configuration;
    using Microsoft.Extensions.Logging;
    using MQTTnet;
    using MQTTnet.Server;
    using Newtonsoft.Json;

    public class MqttServerStorage : IMqttServerStorage
    {
        private readonly ILogger<MqttServerStorage> _logger;
        private readonly List<MqttApplicationMessage> _messages = new List<MqttApplicationMessage>();

        private readonly MqttSettingsModel _mqttSettings;
        private bool _messagesHaveChanged;
        private string _path;

        public MqttServerStorage(MqttSettingsModel mqttSettings, ILogger<MqttServerStorage> logger)
        {
            this._mqttSettings = mqttSettings ?? throw new ArgumentNullException(nameof(mqttSettings));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._path = string.Empty;

            this.Configure();
        }

        public void Configure()
        {
            if (this._mqttSettings.RetainedApplicationMessages?.Persist != true ||
                string.IsNullOrEmpty(this._mqttSettings.RetainedApplicationMessages.Path))
            {
                this._logger.LogInformation("Persisting of retained application messages is disabled.");
                return;
            }

            this._path = PathHelper.ExpandPath(this._mqttSettings.RetainedApplicationMessages.Path) ?? string.Empty;

            // The retained application messages are stored in a separate thread.
            // This is mandatory because writing them to a slow storage (like RaspberryPi SD card)
            // will slow down the whole message processing speed.
            Task.Run(this.SaveRetainedMessagesInternalAsync, CancellationToken.None);
        }

        public async Task<IList<MqttApplicationMessage>?> LoadRetainedMessagesAsync()
        {
            if (this._mqttSettings.RetainedApplicationMessages?.Persist != true || !File.Exists(this._path))
            {
                return null;
            }

            try
            {
                var json = await File.ReadAllTextAsync(this._path).ConfigureAwait(false);
                var applicationMessages = JsonConvert.DeserializeObject<List<MqttApplicationMessage>>(json);

                this._logger.LogInformation($"{applicationMessages.Count} retained MQTT messages loaded.");

                return applicationMessages;
            }
            catch (Exception exception)
            {
                this._logger.LogWarning(exception, "Error while loading persisted retained application messages.");

                return null;
            }
        }

        public Task SaveRetainedMessagesAsync(IList<MqttApplicationMessage> messages)
        {
            lock (this._messages)
            {
                this._messages.Clear();
                this._messages.AddRange(messages);

                this._messagesHaveChanged = true;
            }

            return Task.CompletedTask;
        }

        private async Task SaveRetainedMessagesInternalAsync()
        {
            while (true)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(this._mqttSettings.RetainedApplicationMessages.WriteInterval)).ConfigureAwait(false);

                    List<MqttApplicationMessage> messages;
                    lock (this._messages)
                    {
                        if (!this._messagesHaveChanged)
                        {
                            continue;
                        }

                        messages = new List<MqttApplicationMessage>(this._messages);
                        this._messagesHaveChanged = false;
                    }

                    var json = JsonConvert.SerializeObject(messages);
                    await File.WriteAllTextAsync(this._path, json, Encoding.UTF8).ConfigureAwait(false);

                    this._logger.LogInformation($"{messages.Count} retained MQTT messages written.");
                }
                catch (Exception exception)
                {
                    this._logger.LogError(exception, "Error while writing retained MQTT messages.");
                }
            }
        }
    }
}
