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

        public Task<string> GetUserIdAsync()
        {
            return _http.GetStringAsync($"{baseUrl}/UserId");
        }

        public Task<string> GetUserEmailAsync()
        {
            return _http.GetStringAsync($"{baseUrl}/UserEmail");
        }

        public Task<string> GetPlatformIdAsync()
        {
            return _http.GetStringAsync($"{baseUrl}/PlatformId");
        }

        public Task<string> GetRolesAsync()
        {
            return _http.GetStringAsync($"{baseUrl}/Roles");
        }
    }
}
