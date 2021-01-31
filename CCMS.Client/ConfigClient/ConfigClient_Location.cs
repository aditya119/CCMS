using CCMS.Shared.Models.LocationModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public partial class ConfigClient
    {
        public Task<IEnumerable<LocationDetailsModel>> GetAllLocationsAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<LocationDetailsModel>>(locationBaseUrl);
        }

        public Task<LocationDetailsModel> GetLocationByIdAsync(int locationId)
        {
            return _http.GetFromJsonAsync<LocationDetailsModel>($"{locationBaseUrl}/{locationId}");
        }

        public Task<HttpResponseMessage> AddNewLocationAsync(NewLocationModel locationDetails)
        {
            return _http.PostAsJsonAsync(locationBaseUrl, locationDetails);
        }

        public Task<HttpResponseMessage> UpdateLocationAsync(LocationDetailsModel locationDetails)
        {
            return _http.PutAsJsonAsync(locationBaseUrl, locationDetails);
        }

        public Task<HttpResponseMessage> DeleteLocationAsync(int locationId)
        {
            return _http.DeleteAsync($"{locationBaseUrl}/{locationId}");
        }
    }
}
