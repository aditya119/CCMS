using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("UserId")]
        public ActionResult<int> GetUserId()
        {
            int userId = _sessionService.GetUserId(HttpContext);
            return Ok(userId);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("UserEmail")]
        public ActionResult<string> GetUserEmail()
        {
            string userEmail = _sessionService.GetUserEmail(HttpContext);
            return Ok(userEmail);
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("Roles")]
        public ActionResult<string> GetRoles()
        {
            IEnumerable<string> roles = _sessionService.GetRoles(HttpContext);
            return Ok(string.Join(',', roles));
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Route("PlatformId")]
        public ActionResult<int> GetPlatformId()
        {
            int platformId = _sessionService.GetPlatformId(HttpContext);
            return Ok(platformId);
        }
    }
}
