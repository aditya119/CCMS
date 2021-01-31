using System.Net.Http;

namespace CCMS.Client
{
    public partial class ConfigClient
    {
        private readonly HttpClient _http;
        private readonly string baseUrl = "api/config";
        private readonly string caseTypeBaseUrl;
        private readonly string courtBaseUrl;
        private readonly string lawyerBaseUrl;
        private readonly string locationBaseUrl;

        public ConfigClient(HttpClient http)
        {
            _http = http;
            caseTypeBaseUrl = $"{baseUrl}/CaseType";
            courtBaseUrl = $"{baseUrl}/Court";
            lawyerBaseUrl = $"{baseUrl}/Lawyer";
            locationBaseUrl = $"{baseUrl}/Location";
        }
    }
}
