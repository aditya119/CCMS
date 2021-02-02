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

        public async Task<int> GetUserIdAsync()
        {
            string userId = await _http.GetStringAsync($"{baseUrl}/UserId");
            return int.Parse(userId);
        }

        public Task<string> GetUserEmailAsync()
        {
            return _http.GetStringAsync($"{baseUrl}/UserEmail");
        }

        public async Task<int> GetPlatformIdAsync()
        {
            string platformId = await _http.GetStringAsync($"{baseUrl}/PlatformId");
            return int.Parse(platformId);
        }

        public Task<string> GetRolesAsync()
        {
            return _http.GetStringAsync($"{baseUrl}/Roles");
        }
    }
}
