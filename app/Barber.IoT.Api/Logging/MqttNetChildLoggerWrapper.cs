namespace Barber.IoT.Api.Logging
{
    using System;
    using MQTTnet.Diagnostics;

    public class MqttNetChildLoggerWrapper : IMqttNetChildLogger
    {
        private readonly MqttNetLoggerWrapper _logger;
        private readonly string? _source;

        public MqttNetChildLoggerWrapper(string? source, MqttNetLoggerWrapper logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));

            this._source = source;
        }

        public IMqttNetChildLogger CreateChildLogger(string? source = null)
        {
            return this._logger.CreateChildLogger(source);
        }

        public void Verbose(string message, params object[] parameters)
        {
            this._logger.Publish(MqttNetLogLevel.Verbose, this._source, message, parameters, null);
        }

        public void Info(string message, params object[] parameters)
        {
            this._logger.Publish(MqttNetLogLevel.Info, this._source, message, parameters, null);
        }

        public void Warning(Exception exception, string message, params object[] parameters)
        {
            this._logger.Publish(MqttNetLogLevel.Warning, this._source, message, parameters, exception);
        }

        public void Error(Exception exception, string message, params object[] parameters)
        {
            this._logger.Publish(MqttNetLogLevel.Error, this._source, message, parameters, exception);
        }
    }
}
