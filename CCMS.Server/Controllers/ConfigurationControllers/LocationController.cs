using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.LocationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.ConfigurationControllers
{
    [Route("api/config/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class LocationController : ControllerBase
    {
        private readonly ILocationsService _locationsService;

        public LocationController(ILocationsService locationsService)
        {
            _locationsService = locationsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LocationDetailsModel>>> GetAllLocations()
        {
            IEnumerable<LocationDetailsModel> allLocations = await _locationsService.RetrieveAllAsync();
            return Ok(allLocations);
        }

        [HttpGet]
        [Route("{locationId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<ActionResult<LocationDetailsModel>> GetLocationDetails(int locationId)
        {
            if (locationId < 0)
            {
                return UnprocessableEntity($"Invalid LocationId: {locationId}");
            }
            LocationDetailsModel locationDetails = await _locationsService.RetrieveAsync(locationId);
            if (locationDetails is null)
            {
                return NotFound();
            }
            return Ok(locationDetails);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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
