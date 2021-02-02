using System.Collections.Generic;
using CCMS.Shared.Models.InsightsModels;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class CaseClient
    {
        public Task<PendingDisposedCountModel> GetPendingDisposedCountAsync()
        {
            return _http.GetFromJsonAsync<PendingDisposedCountModel>($"{insightsBaseUrl}/PendingDisposedCount");
        }

        public Task<IEnumerable<ParameterisedReportModel>> GetParameterisedReportAsync(ReportFilterModel reportFilters)
        {
            return _http.GetFromJsonAsync<IEnumerable<ParameterisedReportModel>>($"{insightsBaseUrl}/ParametrisedReport?{reportFilters.QueryString}");
        }
    }
}
