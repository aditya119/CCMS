using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<IEnumerable<CaseActorModel>>> GetCaseActorDetails(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            IEnumerable<CaseActorModel> caseActors = await _caseActorsService.RetrieveAsync(caseId);
            return Ok(caseActors);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
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
