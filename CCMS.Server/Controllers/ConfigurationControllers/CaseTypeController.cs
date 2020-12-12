using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.CaseTypeModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CaseTypeDetailsModel>>> GetAllCaseTypes()
        {
            IEnumerable<CaseTypeDetailsModel> allCaseTypes = await _caseTypesService.RetrieveAllAsync();
            return Ok(allCaseTypes);
        }

        [HttpGet]
        [Route("{caseTypeId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<ActionResult<CaseTypeDetailsModel>> GetCaseTypeDetails(int caseTypeId)
        {
            if (caseTypeId < 0)
            {
                return UnprocessableEntity($"Invalid CaseTypeId: {caseTypeId}");
            }
            CaseTypeDetailsModel caseTypeDetails = await _caseTypesService.RetrieveAsync(caseTypeId);
            if (caseTypeDetails is null)
            {
                NotFound();
            }
            return Ok(caseTypeDetails);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateNewCaseType(NewCaseTypeModel caseTypeModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int caseTypeId = await _caseTypesService.CreateAsync(caseTypeModel);

            return Created("api/config/CaseType", caseTypeId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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
