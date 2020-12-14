using System;
using CCMS.Server.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILoggingService _loggingService;

        public ErrorController(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error()
        {
            Exception error = HttpContext.Features.Get<IExceptionHandlerFeature>().Error;
            _loggingService.LogError(error);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}