using CCMS.Shared.Models.InsightsModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IInsightsService
    {
        Task<IEnumerable<ParameterisedReportModel>> GetParameterisedReportAsync(ReportFilterModel filterModel);
        Task<PendingDisposedCountModel> GetPendingDisposedCountAsync(int userId);
    }
}