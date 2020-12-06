using System.Collections.Generic;
using CCMS.Server.Services;
using CCMS.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AuthenticateSession]
    public class IdentityController : ControllerBase
    {
        private readonly ISessionService _sessionService;

        public IdentityController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("UserId")]
        public ActionResult<int> GetUserId()
        {
            int userId = _sessionService.GetUserId(HttpContext);
            return Ok(userId);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("UserEmail")]
        public ActionResult<string> GetUserEmail()
        {
            string userEmail = _sessionService.GetUserEmail(HttpContext);
            return Ok(userEmail);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("Roles")]
        public ActionResult<string> GetRoles()
        {
            IEnumerable<string> roles = _sessionService.GetRoles(HttpContext);
            return Ok(string.Join(',', roles));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("PlatformId")]
        public ActionResult<int> GetPlatformId()
        {
            int platformId = _sessionService.GetPlatformId(HttpContext);
            return Ok(platformId);
        }
    }
}
