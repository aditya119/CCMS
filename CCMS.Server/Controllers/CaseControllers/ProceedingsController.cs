using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CCMS.Server.Utilities;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
    [AuthenticateSession]
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
            CaseProceedingModel caseProceeding = await _caseProceedingsService
                .RetrieveAsync(caseProceedingId);
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
            IEnumerable<CaseProceedingModel> caseProceedings = await _caseProceedingsService
                .RetrieveAllCaseProceedingsAsync(caseId);
            return Ok(caseProceedings);
        }

        [HttpGet]
        [Route("assigned/{userId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<IEnumerable<AssignedProceedingModel>>> GetAssignedProceedings(int userId)
        {
            if (userId < 0)
            { //0 for all users
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            IEnumerable<AssignedProceedingModel> assignedProceedings = await _caseProceedingsService
                .RetrieveAssignedProceedingsAsync(userId);
            return Ok(assignedProceedings);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> UpdateCaseProceedingDetails(CaseProceedingModel caseProceedingModel)
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

        [HttpDelete]
        [Route("{caseProceedingId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int caseProceedingId)
        {
            if (caseProceedingId < 1)
            {
                return UnprocessableEntity($"Invalid CaseProceedingId: {caseProceedingId}");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _caseProceedingsService.DeleteAsync(caseProceedingId, currUser);
            return NoContent();
        }
    }
}
