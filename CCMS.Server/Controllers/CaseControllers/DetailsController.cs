using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.CourtCaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<CaseDetailsModel>> GetCaseDetails(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            CaseDetailsModel caseDetails = await _courtCasesService.RetrieveAsync(caseId);
            return Ok(caseDetails);
        }

        [HttpGet]
        [Route("{caseId:int}/status")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<(int, string)>> GetCaseStatus(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            (int statusId, string statusStr) = await _courtCasesService.GetCaseStatus(caseId);
            return Ok((statusId, statusStr));
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Operator")]
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
                (_, string statusStr) = await _courtCasesService.GetCaseStatus(prevAppealCaseId);
                if (statusStr != "FINAL JUDGEMENT")
                {
                    return UnprocessableEntity($"Previous appeal status, {statusStr}, must be FINAL JUDGEMENT");
                }
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            int caseId = await _courtCasesService.CreateAsync(caseModel, currUser);
            return Created("api/Case/Details", caseId);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Operator")]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            await _courtCasesService.DeleteAsync(caseId);
            return NoContent();
        }
    }
}
