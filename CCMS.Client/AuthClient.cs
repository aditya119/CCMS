using CCMS.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public class AuthClient
    {
        private readonly HttpClient _http;

        public AuthClient(HttpClient http)
        {
            _http = http;
        }

        public Task<HttpResponseMessage> LoginAsync(LoginModel loginModel)
        {
            return _http.PostAsJsonAsync("api/Login", loginModel);
        }

        public Task<HttpResponseMessage> LogoutAsync()
        {
            return _http.PostAsync("api/Logout", null);
        }
    }
}
