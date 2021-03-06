﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers.FactoryDataControllers
{
    [Route("api/FactoryData/[controller]")]
    [ApiController]
    [Authorize]
    [AuthenticateSession]
    public class ProceedingDecisionsController : ControllerBase
    {
        private readonly IProceedingDecisionsService _proceedingDecisionsService;

        public ProceedingDecisionsController(IProceedingDecisionsService proceedingDecisionsService)
        {
            _proceedingDecisionsService = proceedingDecisionsService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ProceedingDecisionModel>>> GetAllProceedingDecisions()
        {
            IEnumerable<ProceedingDecisionModel> proceedingDecisions = await _proceedingDecisionsService.RetrieveAllAsync();
            return Ok(proceedingDecisions);
        }
    }
}
