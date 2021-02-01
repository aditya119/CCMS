using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public class FactoryDataClient
    {
        private readonly HttpClient _http;
        private readonly string baseUrl = "api/FactoryData";

        public FactoryDataClient(HttpClient http)
        {
            _http = http;
        }

        public Task<IEnumerable<IEnumerable<ActorTypeModel>>> GetAllActorTypesAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<IEnumerable<ActorTypeModel>>>($"{baseUrl}/ActorTypes");
        }

        public Task<IEnumerable<IEnumerable<PlatformModel>>> GetAllPlatformsAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<IEnumerable<PlatformModel>>>($"{baseUrl}/Platforms");
        }

        public Task<IEnumerable<ProceedingDecisionModel>> GetAllProceedingDecisionsAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<ProceedingDecisionModel>>($"{baseUrl}/ProceedingDecisions");
        }

        public Task<IEnumerable<RoleModel>> GetAllRolesAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<RoleModel>>($"{baseUrl}/Roles");
        }

        public Task<string> GetRolesFromCsvStringAsync(string rolesCsv)
        {
            return _http.GetStringAsync($"{baseUrl}/Roles/csv?rolesCsv={rolesCsv}");
        }
    }
}
