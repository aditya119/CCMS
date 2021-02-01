using CCMS.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class CaseClient
    {
        public Task<CaseDatesModel> GetCaseDatesAsync(int caseId)
        {
            return _http.GetFromJsonAsync<CaseDatesModel>($"{datesBaseUrl}/{caseId}");
        }

        public Task<HttpResponseMessage> UpdateCaseDatesAsync(CaseDatesModel caseDates)
        {
            return _http.PostAsJsonAsync($"{datesBaseUrl}", caseDates);
        }
    }
}
