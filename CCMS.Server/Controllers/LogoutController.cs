﻿using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LogoutController : ControllerBase
    {
        private readonly ISessionService _sessionService;
        private readonly IAuthService _authService;

        public LogoutController(ISessionService sessionService, IAuthService authService)
        {
            _sessionService = sessionService;
            _authService = authService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post()
        {
            int userId = _sessionService.GetUserId(HttpContext);
            int platformId = _sessionService.GetPlatformId(HttpContext);
            await _authService.LogoutAsync(userId, platformId);
            return NoContent();
        }
    }
}
