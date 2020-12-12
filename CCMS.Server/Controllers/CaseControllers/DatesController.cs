using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Server.Utilities;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class DatesController : ControllerBase
    {
        private readonly ICaseDatesService _caseDatesService;
        private readonly ISessionService _sessionService;

        public DatesController(ICaseDatesService caseDatesService,
            ISessionService sessionService)
        {
            _caseDatesService = caseDatesService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [Route("{caseId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<CaseDatesModel>> GetCaseDateDetails(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            CaseDatesModel caseDates = await _caseDatesService.RetrieveAsync(caseId);
            if (caseDates is null)
            {
                return NotFound();
            }
            return Ok(caseDates);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> UpdateCaseDateDetails(CaseDatesModel caseDatesModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _caseDatesService.UpdateAsync(caseDatesModel, currUser);

            return NoContent();
        }
    }
}
