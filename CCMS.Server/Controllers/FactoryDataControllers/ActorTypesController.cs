using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.FactoryDataControllers
{
    [Route("api/FactoryData/[controller]")]
    [ApiController]
    [Authorize]
    [AuthenticateSession]
    public class ActorTypesController : ControllerBase
    {
        private readonly IActorTypesService _actorTypesService;

        public ActorTypesController(IActorTypesService actorTypesService)
        {
            _actorTypesService = actorTypesService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<IEnumerable<ActorTypeModel>>> GetAllActorTypes()
        {
            IEnumerable<ActorTypeModel> actorTypes = await _actorTypesService.RetrieveAllAsync();
            return Ok(actorTypes);
        }
    }
}
