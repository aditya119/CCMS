using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.CourtModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.ConfigurationControllers
{
    [Route("api/config/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class CourtController : ControllerBase
    {
        private readonly ICourtsService _courtsService;

        public CourtController(ICourtsService courtsService)
        {
            _courtsService = courtsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CourtDetailsModel>>> GetAllCourts()
        {
            IEnumerable<CourtDetailsModel> allCourts = await _courtsService.RetrieveAllAsync();
            return Ok(allCourts);
        }

        [HttpGet]
        [Route("{courtId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<ActionResult<CourtDetailsModel>> GetCourtDetails(int courtId)
        {
            if (courtId < 0)
            {
                return UnprocessableEntity($"Invalid CourtId: {courtId}");
            }
            CourtDetailsModel courtDetails = await _courtsService.RetrieveAsync(courtId);
            if (courtDetails is null)
            {
                return NotFound();
            }
            return Ok(courtDetails);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateNewCourt(NewCourtModel courtModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int courtId = await _courtsService.CreateAsync(courtModel);

            return Created("api/config/Court", courtId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateCourtDetails(CourtDetailsModel courtModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }

            await _courtsService.UpdateAsync(courtModel);
            return NoContent();
        }

        [HttpDelete]
        [Route("{courtId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int courtId)
        {
            if (courtId < 1)
            {
                return UnprocessableEntity($"Invalid CourtId: {courtId}");
            }
            await _courtsService.DeleteAsync(courtId);
            return NoContent();
        }
    }
}
