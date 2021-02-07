using System;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.CourtCaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CCMS.Shared.Enums;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class DetailsController : ControllerBase
    {
        private readonly ICourtCasesService _courtCasesService;
        private readonly ISessionService _sessionService;

        public DetailsController(ICourtCasesService courtCasesService,
            ISessionService sessionService)
        {
            _courtCasesService = courtCasesService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("{caseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Operator)]
        public async Task<ActionResult<CaseDetailsModel>> GetCaseDetails(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            CaseDetailsModel caseDetails = await _courtCasesService.RetrieveAsync(caseId);
            if (caseDetails is null)
            {
                return NotFound();
            }
            return Ok(caseDetails);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Operator)]
        public async Task<ActionResult<int>> GetCaseId(string caseNumber, int appealNumber)
        { // Todo: Unit Test
            if (string.IsNullOrWhiteSpace(caseNumber) || caseNumber.Length > 1000 || appealNumber < 0)
            {
                return UnprocessableEntity("Invalid Parameters");
            }
            int caseId = -1; // current (caseNumber, appealNumber) does not exist
            (int existingCaseId, DateTime? deleted) = await _courtCasesService
                .ExistsCaseNumberAsync(caseNumber, appealNumber);
            if (existingCaseId != -1 && deleted.HasValue == false)
            {
                caseId = existingCaseId;
            }
            if (appealNumber > 0 && caseId == -1)
            {
                (int prevAppealCaseId, DateTime? prevAppealDeleted) = await _courtCasesService
                    .ExistsCaseNumberAsync(caseNumber, appealNumber - 1);
                if (prevAppealCaseId == -1 || prevAppealDeleted.HasValue)
                {// cannot create new appeal because previous appeal does not exist
                    caseId = -2;
                }
                else
                {
                    CaseStatusModel caseStatus = await _courtCasesService.GetCaseStatusAsync(prevAppealCaseId);
                    if (caseStatus.StatusName != ProceedingDecisions.FinalJudgement)
                    {// previous appeal must be final judgement
                        caseId = -3;
                    }
                }
            }
            return Ok(caseId);
        }

        [HttpGet]
        [Route("{caseId:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Operator)]
        public async Task<ActionResult<CaseStatusModel>> GetCaseStatus(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            CaseStatusModel caseStatus = await _courtCasesService.GetCaseStatusAsync(caseId);
            if (caseStatus is null)
            {
                return NotFound();
            }
            return Ok(caseStatus);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Operator)]
        public async Task<IActionResult> CreateNewCase(NewCaseModel caseModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            (int existingCaseId, DateTime? deleted) = await _courtCasesService
                .ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber);
            if (existingCaseId != -1 && deleted.HasValue == false)
            {
                return UnprocessableEntity("Case already exists");
            }
            if (caseModel.AppealNumber > 0)
            {
                (int prevAppealCaseId, DateTime? prevAppealDeleted) = await _courtCasesService
                    .ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber - 1);
                if (prevAppealCaseId == -1 || prevAppealDeleted.HasValue)
                {
                    return UnprocessableEntity("Previous appeal does not exist");
                }
                CaseStatusModel caseStatus = await _courtCasesService.GetCaseStatusAsync(prevAppealCaseId);
                if (caseStatus.StatusName != ProceedingDecisions.FinalJudgement)
                {
                    return UnprocessableEntity($"Previous appeal status, {caseStatus.StatusName}, must be {ProceedingDecisions.FinalJudgement}");
                }
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            int caseId = await _courtCasesService.CreateAsync(caseModel, currUser); // restore case if deleted
            return Created("api/Case/Details", caseId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Operator)]
        public async Task<IActionResult> UpdateCaseDetails(UpdateCaseModel caseModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            (string caseNumber, _, DateTime? deleted) = await _courtCasesService
                .ExistsCaseIdAsync(caseModel.CaseId);
            if (string.IsNullOrEmpty(caseNumber) || deleted.HasValue)
            {
                return UnprocessableEntity("Case deleted or does not exist");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _courtCasesService.UpdateAsync(caseModel, currUser);
            return NoContent();
        }

        [HttpDelete]
        [Route("{caseId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<IActionResult> Delete(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _courtCasesService.DeleteAsync(caseId, currUser);
            return NoContent();
        }
    }
}
