namespace Barber.IoT.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Main API Base Controller
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    public abstract class ApiBaseController : ControllerBase
    {
        /// <summary>
        /// Base Constructor
        /// </summary>
        public ApiBaseController()
        {
        }
    }
}
