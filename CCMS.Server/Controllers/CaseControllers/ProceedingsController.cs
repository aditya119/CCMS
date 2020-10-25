using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Shared.Models;
using CCMS.Shared.Models.CaseProceedingModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
    public class ProceedingsController : ControllerBase
    {
        private readonly ICaseProceedingsService _caseProceedingsService;
        private readonly IProceedingDecisionsService _proceedingDecisionsService;
        private readonly ISessionService _sessionService;

        public ProceedingsController(ICaseProceedingsService caseProceedingsService,
            IProceedingDecisionsService proceedingDecisionsService,
            ISessionService sessionService)
        {
            _caseProceedingsService = caseProceedingsService;
            _proceedingDecisionsService = proceedingDecisionsService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<CaseProceedingModel>> GetProceedingDetails(int caseProceedingId)
        {
            if (caseProceedingId < 1)
            {
                return UnprocessableEntity($"Invalid CaseProceedingId: {caseProceedingId}");
            }
            CaseProceedingModel caseProceeding = await _caseProceedingsService.RetrieveAsync(caseProceedingId);
            return Ok(caseProceeding);
        }

        [HttpGet]
        [Route("{caseId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<IEnumerable<CaseProceedingModel>>> GetCaseProceedings(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            IEnumerable<CaseProceedingModel> caseProceedings = await _caseProceedingsService.RetrieveAllCaseProceedingsAsync(caseId);
            return Ok(caseProceedings);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> UpdateCaseProceedingDetails(UpdateCaseProceedingModel caseProceedingModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            ProceedingDecisionModel proceedingDecision = await _proceedingDecisionsService.RetrieveAsync(caseProceedingModel.ProceedingDecision);
            if ((proceedingDecision.HasNextHearingDate && caseProceedingModel.NextHearingOn.HasValue) == false
                || (proceedingDecision.HasOrderAttachment && caseProceedingModel.JudgementFile != 0) == false)
            {
                return UnprocessableEntity("Proceeding Decision conditions not met");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _caseProceedingsService.UpdateAsync(caseProceedingModel, currUser);

            return NoContent();
        }

        [HttpPost]
        [Route("{caseId:int}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> AssignCaseProceeding(int caseId, int assignTo)
        {
            int currUser = _sessionService.GetUserId(HttpContext);
            await _caseProceedingsService.AssignProceedingAsync(caseId, assignTo, currUser);

            return NoContent();
        }
    }
}
