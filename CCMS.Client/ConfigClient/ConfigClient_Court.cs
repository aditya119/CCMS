using CCMS.Shared.Models.CourtModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class ConfigClient
    {
        public Task<IEnumerable<CourtDetailsModel>> GetAllCourtsAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<CourtDetailsModel>>(courtBaseUrl);
        }

        public Task<CourtDetailsModel> GetCourtByIdAsync(int courtId)
        {
            return _http.GetFromJsonAsync<CourtDetailsModel>($"{courtBaseUrl}/{courtId}");
        }

        public Task<HttpResponseMessage> AddNewCourtAsync(NewCourtModel courtDetails)
        {
            return _http.PostAsJsonAsync(courtBaseUrl, courtDetails);
        }

        public Task<HttpResponseMessage> UpdateCourtAsync(CourtDetailsModel courtDetails)
        {
            return _http.PutAsJsonAsync(courtBaseUrl, courtDetails);
        }

        public Task<HttpResponseMessage> DeleteCourtAsync(int courtId)
        {
            return _http.DeleteAsync($"{courtBaseUrl}/{courtId}");
        }
    }
}
