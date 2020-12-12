using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CCMS.Server.Utilities;
using Microsoft.AspNetCore.Http;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class ActorsController : ControllerBase
    {
        private readonly ICaseActorsService _caseActorsService;
        private readonly ISessionService _sessionService;

        public ActorsController(ICaseActorsService caseActorsService,
            ISessionService sessionService)
        {
            _caseActorsService = caseActorsService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("{caseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<IEnumerable<CaseActorModel>>> GetCaseActorDetails(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            IEnumerable<CaseActorModel> caseActors = await _caseActorsService.RetrieveAsync(caseId);
            if (caseActors is null)
            {
                return NotFound();
            }
            return Ok(caseActors);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> UpdateCaseActorDetails(IEnumerable<CaseActorModel> caseActorModels)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _caseActorsService.UpdateAsync(caseActorModels, currUser);

            return NoContent();
        }
    }
}
