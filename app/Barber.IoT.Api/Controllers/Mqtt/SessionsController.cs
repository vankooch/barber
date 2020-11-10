namespace Barber.IoT.Api.Controllers.Mqtt
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using System.Web;
    using Barber.IoT.Data.Model;
    using Barber.IoT.MQTTNet;
    using Microsoft.AspNetCore.Mvc;
    using MQTTnet.Server.Status;

    [Route("api/mqtt/[controller]")]
    public class SessionsController : ApiBaseController
    {
        private readonly MqttService<Device> _mqttServerService;

        public SessionsController(MqttService<Device> mqttServerService)
            => this._mqttServerService = mqttServerService;

        [HttpDelete("pending/{clientId}")]
        public async Task<ActionResult> DeletePendingApplicationMessages(string clientId)
        {
            clientId = HttpUtility.UrlDecode(clientId);

            var session = (await this._mqttServerService.Server.GetSessionStatusAsync()).FirstOrDefault(c => c.ClientId == clientId);
            if (session == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            await session.ClearPendingApplicationMessagesAsync();

            return this.Ok();
        }

        [HttpDelete("{clientId}")]
        public async Task<ActionResult> DeleteSession(string clientId)
        {
            clientId = HttpUtility.UrlDecode(clientId);

            var session = (await this._mqttServerService.Server.GetSessionStatusAsync()).FirstOrDefault(c => c.ClientId == clientId);
            if (session == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            await session.DeleteAsync();
            return this.StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet("{clientId}")]
        public async Task<ActionResult<IMqttClientStatus>> GetSession(string clientId)
        {
            clientId = HttpUtility.UrlDecode(clientId);

            var session = (await this._mqttServerService.Server.GetSessionStatusAsync()).FirstOrDefault(c => c.ClientId == clientId);
            if (session == null)
            {
                return new StatusCodeResult((int)HttpStatusCode.NotFound);
            }

            return new ObjectResult(session);
        }

        [HttpGet]
        public async Task<ActionResult<IList<IMqttSessionStatus>>> GetSessions()
        {
            return new ObjectResult(await this._mqttServerService.Server.GetSessionStatusAsync());
        }
    }
}
