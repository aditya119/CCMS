using System.Net.Http;

namespace CCMS.Client
{
    public partial class CaseClient
    {
        private readonly HttpClient _http;
        private readonly string baseUrl = "api/Case";
        private readonly string detailsBaseUrl;
        private readonly string datesBaseUrl;
        private readonly string actorsBaseUrl;
        private readonly string proceedingsBaseUrl;
        private readonly string insightsBaseUrl;

        public CaseClient(HttpClient http)
        {
            _http = http;
            detailsBaseUrl = $"{baseUrl}/Details";
            datesBaseUrl = $"{baseUrl}/Dates";
            actorsBaseUrl = $"{baseUrl}/Actors";
            proceedingsBaseUrl = $"{baseUrl}/Proceedings";
            insightsBaseUrl = $"{baseUrl}/Insights";
        }
    }
}
