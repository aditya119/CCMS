using CCMS.Client;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using System.Threading.Tasks;

namespace CCMS.Web.Authenitication
{
    public class AuthService : IAuthService
    {
        private readonly AuthClient _authClient;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(AuthClient authClient, AuthenticationStateProvider authenticationStateProvider)
        {
            _authClient = authClient;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> LoginAsync(LoginModel loginModel)
        {
            HttpResponseMessage response = await _authClient.LoginAsync(loginModel);

            if (response.IsSuccessStatusCode == false)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(errorContent))
                {
                    errorContent = response.StatusCode.ToString();
                }
                return errorContent;
            }
            string tokenString = await response.Content.ReadAsStringAsync();

            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(tokenString);

            return null;
        }

        public async Task LogoutAsync()
        {
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }
    }
}
