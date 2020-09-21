using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public IdentityController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("UserId")]
        public async Task<ActionResult<int>> GetUserId()
        {
            if (await _sessionService.IsValidSessionAsync(HttpContext))
            {
                int userId = _sessionService.GetUserId(HttpContext);
                return Ok(userId);
            }
            return Unauthorized();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("UserEmail")]
        public async Task<ActionResult<string>> GetUserEmail()
        {
            if (await _sessionService.IsValidSessionAsync(HttpContext))
            {
                string userEmail = _sessionService.GetUserEmail(HttpContext);
                return Ok(userEmail);
            }
            return Unauthorized();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("Roles")]
        public async Task<ActionResult<string>> GetRoles()
        {
            if (await _sessionService.IsValidSessionAsync(HttpContext))
            {
                IEnumerable<string> roles = _sessionService.GetRoles(HttpContext);
                return Ok(string.Join(',', roles));
            }
            return Unauthorized();
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("PlatformId")]
        public async Task<ActionResult<int>> GetPlatformId()
        {
            if (await _sessionService.IsValidSessionAsync(HttpContext))
            {
                int platformId = _sessionService.GetPlatformId(HttpContext);
                return Ok(platformId);
            }
            return Unauthorized();
        }
    }
}
