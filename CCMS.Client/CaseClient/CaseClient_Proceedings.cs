using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class CaseClient
    {
        public Task<IEnumerable<CaseProceedingModel>> GetAllCaseProceedingsAsync(int caseId)
        {
            return _http.GetFromJsonAsync<IEnumerable<CaseProceedingModel>>($"{proceedingsBaseUrl}/{caseId}");
        }

        public Task<CaseProceedingModel> GetCaseProceedingByIdAsync(int caseProceedingId)
        {
            return _http.GetFromJsonAsync<CaseProceedingModel>($"{proceedingsBaseUrl}?caseProceedingId={caseProceedingId}");
        }

        public Task<IEnumerable<PendingProceedingModel>> GetPendingProceedingsAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<PendingProceedingModel>>($"{proceedingsBaseUrl}/pending");
        }

        public Task<HttpResponseMessage> UpdateCaseProceedingAsync(CaseProceedingModel caseProceedingModel)
        {
            return _http.PostAsJsonAsync($"{proceedingsBaseUrl}", caseProceedingModel);
        }

        public Task<HttpResponseMessage> AssignCaseProceedingAsync(int caseProceedingId, int assignTo)
        {
            return _http.PostAsync($"{proceedingsBaseUrl}/{caseProceedingId}?assignTo={assignTo}", null);
        }
    }
}
