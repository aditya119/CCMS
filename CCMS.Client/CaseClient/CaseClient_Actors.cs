using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class CaseClient
    {
        public Task<IEnumerable<CaseActorModel>> GetCaseActorsAsync(int caseId)
        {
            return _http.GetFromJsonAsync<IEnumerable<CaseActorModel>>($"{actorsBaseUrl}/{caseId}");
        }

        public Task<HttpResponseMessage> UpdateCaseActorsAsync(IEnumerable<CaseActorModel> caseActors)
        {
            return _http.PostAsJsonAsync($"{actorsBaseUrl}", caseActors);
        }
    }
}
