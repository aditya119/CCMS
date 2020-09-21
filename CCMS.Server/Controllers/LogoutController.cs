using System.Threading.Tasks;
using CCMS.Server.DbServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogoutController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public LogoutController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Post()
        {
            await _sessionService.ClearSessionAsync(HttpContext);
            return NoContent();
        }
    }
}
