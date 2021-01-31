using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public class IdentityClient
    {
        private readonly HttpClient _http;
        private readonly string baseUrl = "api/Identity";

        public IdentityClient(HttpClient http)
        {
            _http = http;
        }

        public Task<int> GetUserIdAsync()
        {
            return _http.GetFromJsonAsync<int>($"{baseUrl}/UserId");
        }

        public Task<string> GetUserEmailAsync()
        {
            return _http.GetFromJsonAsync<string>($"{baseUrl}/UserEmail");
        }

        public Task<int> GetPlatformIdAsync()
        {
            return _http.GetFromJsonAsync<int>($"{baseUrl}/PlatformId");
        }

        public Task<string> GetRolesAsync()
        {
            return _http.GetFromJsonAsync<string>($"{baseUrl}/Roles");
        }
    }
}
