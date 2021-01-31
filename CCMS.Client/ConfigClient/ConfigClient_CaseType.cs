using CCMS.Shared.Models.CaseTypeModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class ConfigClient
    {
        public Task<IEnumerable<CaseTypeDetailsModel>> GetAllCaseTypesAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<CaseTypeDetailsModel>>(caseTypeBaseUrl);
        }

        public Task<CaseTypeDetailsModel> GetCaseTypeByIdAsync(int caseTypeId)
        {
            return _http.GetFromJsonAsync<CaseTypeDetailsModel>($"{caseTypeBaseUrl}/{caseTypeId}");
        }

        public Task<HttpResponseMessage> AddNewCaseTypeAsync(NewCaseTypeModel caseTypeDetails)
        {
            return _http.PostAsJsonAsync(caseTypeBaseUrl, caseTypeDetails);
        }

        public Task<HttpResponseMessage> UpdateCaseTypeAsync(CaseTypeDetailsModel caseTypeDetails)
        {
            return _http.PutAsJsonAsync(caseTypeBaseUrl, caseTypeDetails);
        }

        public Task<HttpResponseMessage> DeleteCaseTypeAsync(int caseTypeId)
        {
            return _http.DeleteAsync($"{caseTypeBaseUrl}/{caseTypeId}");
        }
    }
}
