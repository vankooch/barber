namespace Barber.IoT.Api.Controllers.Mqtt
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Barber.IoT.Api.Mqtt;
    using Microsoft.AspNetCore.Mvc;
    using MQTTnet.Server.Status;

    [Route("api/mqtt/[controller]")]
    public class ClientsController : ApiBaseController
    {
        private readonly MqttServerService _mqttServerService;

        public ClientsController(MqttServerService mqttServerService)
            => this._mqttServerService = mqttServerService;

        [HttpDelete("{clientId}")]
        public async Task<ActionResult> DeleteClient(string clientId)
        {
            clientId = HttpUtility.UrlDecode(clientId);

            var client = (await this._mqttServerService.GetClientStatusAsync()).FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            await client.DisconnectAsync();
            return this.StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpDelete("{clientId}/statistics")]
        public async Task<ActionResult> DeleteClientStatistics(string clientId)
        {
            clientId = HttpUtility.UrlDecode(clientId);

            var client = (await this._mqttServerService.GetClientStatusAsync()).FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            client.ResetStatistics();
            return this.StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet("{clientId}")]
        public async Task<ActionResult<IMqttClientStatus>> GetClient(string clientId)
        {
            clientId = HttpUtility.UrlDecode(clientId);

            var client = (await this._mqttServerService.GetClientStatusAsync()).FirstOrDefault(c => c.ClientId == clientId);
            if (client == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            return new ObjectResult(client);
        }

        [HttpGet]
        public async Task<ActionResult<IList<IMqttClientStatus>>> GetClients()
        {
            return new ObjectResult(await this._mqttServerService.GetClientStatusAsync());
        }
    }
}
