using CCMS.Shared.Models.LawyerModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class ConfigClient
    {
        public Task<IEnumerable<LawyerDetailsModel>> GetAllLawyersAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<LawyerDetailsModel>>(lawyerBaseUrl);
        }

        public Task<LawyerDetailsModel> GetLawyerByIdAsync(int lawyerId)
        {
            return _http.GetFromJsonAsync<LawyerDetailsModel>($"{lawyerBaseUrl}/{lawyerId}");
        }

        public Task<HttpResponseMessage> AddNewLawyerAsync(NewLawyerModel lawyerDetails)
        {
            return _http.PostAsJsonAsync(lawyerBaseUrl, lawyerDetails);
        }

        public Task<HttpResponseMessage> UpdateLawyerAsync(LawyerDetailsModel lawyerDetails)
        {
            return _http.PutAsJsonAsync(lawyerBaseUrl, lawyerDetails);
        }

        public Task<HttpResponseMessage> DeleteLawyerAsync(int lawyerId)
        {
            return _http.DeleteAsync($"{lawyerBaseUrl}/{lawyerId}");
        }
    }
}
