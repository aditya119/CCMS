using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.CaseDatesModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Operator")]
        public async Task<ActionResult<CaseDatesModel>> GetCaseDateDetails(int caseId)
        {
            if (caseId < 1)
            {
                return UnprocessableEntity($"Invalid CaseId: {caseId}");
            }
            CaseDatesModel caseDates = await _caseDatesService.RetrieveAsync(caseId);
            return Ok(caseDates);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Operator")]
        public async Task<IActionResult> UpdateCaseDateDetails(UpdateCaseDatesModel caseDatesModel)
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
