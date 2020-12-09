using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.FactoryDataControllers
{
    [Route("api/FactoryData/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformsService _platformsService;

        public PlatformsController(IPlatformsService platformsService)
        {
            _platformsService = platformsService;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PlatformModel>>> GetAllPlatforms()
        {
            IEnumerable<PlatformModel> platforms = await _platformsService.RetrieveAllAsync();
            return Ok(platforms);
        }
    }
}
