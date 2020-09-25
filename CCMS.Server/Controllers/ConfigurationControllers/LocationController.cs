using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Shared.Models.LocationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.ConfigurationControllers
{
    [Route("api/config/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationsService _locationsService;

        public LocationController(ILocationsService locationsService)
        {
            _locationsService = locationsService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LocationDetailsModel>>> GetAllLocations()
        {
            IEnumerable<LocationDetailsModel> allLocations = await _locationsService.RetrieveAllAsync();
            return Ok(allLocations);
        }

        [HttpGet]
        [Route("{locationId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<LocationDetailsModel>> GetLocationDetails(int locationId)
        {
            if (locationId < 1)
            {
                return UnprocessableEntity($"Invalid LocationId: {locationId}");
            }
            LocationDetailsModel locationDetails = await _locationsService.RetrieveAsync(locationId);
            return Ok(locationDetails);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateNewCourt(NewLocationModel locationModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int locationId = await _locationsService.CreateAsync(locationModel);

            return Created("api/config/Location", locationId);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateCourtDetails(LocationDetailsModel locationModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }

            await _locationsService.UpdateAsync(locationModel);
            return NoContent();
        }

        [HttpDelete]
        [Route("{locationId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int locationId)
        {
            if (locationId < 1)
            {
                return UnprocessableEntity($"Invalid LocationId: {locationId}");
            }
            await _locationsService.DeleteAsync(locationId);
            return NoContent();
        }
    }
}
