using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Shared.Models.CourtModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.ConfigurationControllers
{
    [Route("api/config/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly ICourtsService _courtsService;

        public CourtController(ICourtsService courtsService)
        {
            _courtsService = courtsService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CourtDetailsModel>>> GetAllCourts()
        {
            IEnumerable<CourtDetailsModel> allCourts = await _courtsService.RetrieveAllAsync();
            return Ok(allCourts);
        }

        [HttpGet]
        [Route("{courtId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<CourtDetailsModel>> GetCourtDetails(int courtId)
        {
            if (courtId < 1)
            {
                return UnprocessableEntity($"Invalid CourtId: {courtId}");
            }
            CourtDetailsModel courtDetails = await _courtsService.RetrieveAsync(courtId);
            return Ok(courtDetails);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
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
