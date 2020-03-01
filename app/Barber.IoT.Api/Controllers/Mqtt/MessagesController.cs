﻿namespace Barber.IoT.Api.Controllers.Mqtt
{
    using System.IO;
    using System.Threading.Tasks;
    using Barber.IoT.Data.Model;
    using Barber.IoT.MQTTNet;
    using Microsoft.AspNetCore.Mvc;
    using MQTTnet;
    using MQTTnet.Protocol;

    [Route("api/mqtt/[controller]")]

    public class MessagesController : ApiBaseController
    {
        private readonly MqttService<Device> _mqttServerService;

        public MessagesController(MqttService<Device> mqttServerService)
            => this._mqttServerService = mqttServerService;

        [HttpPost]
        public async Task<ActionResult> PostMessage(MqttApplicationMessage message)
        {
            await this._mqttServerService.Server.PublishAsync(message, default);
            return this.Ok();
        }

        [HttpPost("{*topic}")]
        public async Task<ActionResult> PostMessage(string topic, int qosLevel = 0)
        {
            byte[] payload;

            using (var memoryStream = new MemoryStream())
            {
                await this.HttpContext.Request.Body.CopyToAsync(memoryStream);
                payload = memoryStream.ToArray();
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithQualityOfServiceLevel((MqttQualityOfServiceLevel)qosLevel)
                .Build();

            return await this.PostMessage(message);
        }
    }
}