using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Enums;
using CCMS.Shared.Models.LawyerModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.ConfigurationControllers
{
    [Route("api/config/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class LawyerController : ControllerBase
    {
        private readonly ILawyersService _lawyersService;

        public LawyerController(ILawyersService lawyersService)
        {
            _lawyersService = lawyersService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LawyerListItemModel>>> GetAllLawyers()
        {
            IEnumerable<LawyerListItemModel> allLawyers = await _lawyersService.RetrieveAllAsync();
            return Ok(allLawyers);
        }

        [HttpGet]
        [Route("{lawyerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<ActionResult<LawyerDetailsModel>> GetLawyerDetails(int lawyerId)
        {
            if (lawyerId < 0)
            {
                return UnprocessableEntity($"Invalid LawyerId: {lawyerId}");
            }
            LawyerDetailsModel lawyerDetails = await _lawyersService.RetrieveAsync(lawyerId);
            if (lawyerDetails is null)
            {
                return NotFound();
            }
            return Ok(lawyerDetails);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateNewLawyer(NewLawyerModel lawyerModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int lawyerId = await _lawyersService.CreateAsync(lawyerModel);

            return Created("api/config/Lawyer", lawyerId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UpdateLawyerDetails(LawyerDetailsModel lawyerModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }

            await _lawyersService.UpdateAsync(lawyerModel);
            return NoContent();
        }

        [HttpDelete]
        [Route("{lawyerId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Delete(int lawyerId)
        {
            if (lawyerId < 1)
            {
                return UnprocessableEntity($"Invalid LawyerId: {lawyerId}");
            }
            await _lawyersService.DeleteAsync(lawyerId);
            return NoContent();
        }
    }
}
