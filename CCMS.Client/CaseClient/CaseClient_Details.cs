using CCMS.Shared.Models.CourtCaseModels;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class CaseClient
    {
        public async Task<int> GetCaseIdAsync(string caseNumber, int appealNumber)
        {
            string caseId = await _http.GetStringAsync($"{detailsBaseUrl}?caseNumber={caseNumber}&appealNumber={appealNumber}");
            return int.Parse(caseId);
        }

        public Task<CaseDetailsModel> GetCaseByIdAsync(int caseId)
        {
            return _http.GetFromJsonAsync<CaseDetailsModel>($"{detailsBaseUrl}/{caseId}");
        }

        public Task<CaseStatusModel> GetCaseStatusAsync(int caseId)
        {
            return _http.GetFromJsonAsync<CaseStatusModel>($"{detailsBaseUrl}/{caseId}/status");
        }

        public Task<HttpResponseMessage> AddNewCaseAsync(NewCaseModel caseDetails)
        {
            return _http.PostAsJsonAsync($"{detailsBaseUrl}", caseDetails);
        }

        public Task<HttpResponseMessage> UpdateCaseAsync(CaseDetailsModel caseDetails)
        {
            return _http.PutAsJsonAsync($"{detailsBaseUrl}", caseDetails);
        }

        public Task<HttpResponseMessage> DeleteCaseAsync(int caseId)
        {
            return _http.DeleteAsync($"{detailsBaseUrl}/{caseId}");
        }
    }
}
