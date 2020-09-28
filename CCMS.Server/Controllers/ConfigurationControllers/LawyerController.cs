using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.LawyerModels;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<LawyerListItemModel>>> GetAllLawyers()
        {
            IEnumerable<LawyerListItemModel> allLawyers = await _lawyersService.RetrieveAllAsync();
            return Ok(allLawyers);
        }

        [HttpGet]
        [Route("{lawyerId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<LawyerDetailsModel>> GetLawyerDetails(int lawyerId)
        {
            if (lawyerId < 0)
            {
                return UnprocessableEntity($"Invalid LawyerId: {lawyerId}");
            }
            LawyerDetailsModel lawyerDetails = await _lawyersService.RetrieveAsync(lawyerId);
            return Ok(lawyerDetails);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
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
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Administrator")]
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
