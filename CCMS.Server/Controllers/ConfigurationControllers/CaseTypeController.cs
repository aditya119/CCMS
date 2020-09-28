using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.CaseTypeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.ConfigurationControllers
{
    [Route("api/config/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class CaseTypeController : ControllerBase
    {
        private readonly ICaseTypesService _caseTypesService;

        public CaseTypeController(ICaseTypesService caseTypesService)
        {
            _caseTypesService = caseTypesService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CaseTypeDetailsModel>>> GetAllCaseTypes()
        {
            IEnumerable<CaseTypeDetailsModel> allCaseTypes = await _caseTypesService.RetrieveAllAsync();
            return Ok(allCaseTypes);
        }

        [HttpGet]
        [Route("{caseTypeId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<CaseTypeDetailsModel>> GetCaseTypeDetails(int caseTypeId)
        {
            if (caseTypeId < 0)
            {
                return UnprocessableEntity($"Invalid CaseTypeId: {caseTypeId}");
            }
            CaseTypeDetailsModel caseTypeDetails = await _caseTypesService.RetrieveAsync(caseTypeId);
            return Ok(caseTypeDetails);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateNewLawyer(CaseTypeDetailsModel caseTypeModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int caseTypeId = await _caseTypesService.CreateAsync(caseTypeModel);

            return Created("api/config/CaseType", caseTypeId);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateCaseTypeDetails(CaseTypeDetailsModel caseTypeModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }

            await _caseTypesService.UpdateAsync(caseTypeModel);
            return NoContent();
        }

        [HttpDelete]
        [Route("{caseTypeId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int caseTypeId)
        {
            if (caseTypeId < 1)
            {
                return UnprocessableEntity($"Invalid CaseTypeId: {caseTypeId}");
            }
            await _caseTypesService.DeleteAsync(caseTypeId);
            return NoContent();
        }
    }
}
