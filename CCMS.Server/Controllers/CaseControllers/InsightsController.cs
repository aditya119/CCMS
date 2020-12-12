using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Shared.Models.InsightsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CCMS.Server.Utilities;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace CCMS.Server.Controllers.CaseControllers
{
    [Route("api/Case/[controller]")]
    [ApiController]
    [Authorize]
    [AuthenticateSession]
    public class InsightsController : ControllerBase
    {
        private readonly IInsightsService _insightsService;
        private readonly ISessionService _sessionService;

        public InsightsController(IInsightsService insightsService, ISessionService sessionService)
        {
            _insightsService = insightsService;
            _sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("PendingDisposedCount")]
        public async Task<ActionResult<PendingDisposedCountModel>> GetPendingDisposedCount()
        {
            int userId = _sessionService.IsInRoles(HttpContext, "Manager") ? 0 : _sessionService.GetUserId(HttpContext);
            PendingDisposedCountModel pendingDisposedCountModel = await _insightsService.GetPendingDisposedCountAsync(userId);
            return Ok(pendingDisposedCountModel);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Route("ParametrisedReport")]
        public async Task<ActionResult<IEnumerable<ParameterisedReportModel>>> GetParametrisedReport(ReportFilterModel filterModel)
        {
            IEnumerable<ParameterisedReportModel> reportModel = await _insightsService.GetParameterisedReportAsync(filterModel);
            return Ok(reportModel);
        }
    }
}
