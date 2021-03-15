using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CCMS.Web.Authenitication
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly string jwtTokenKey = "ccmsAuthToken";
        private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));
        public ApiAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var savedToken = await _localStorage.GetItemAsync<string>(jwtTokenKey);

            if (string.IsNullOrWhiteSpace(savedToken))
            { // token does not exist
                return _anonymous;
            }
            var jwtClaims = JwtParser.ParseClaimsFromJwt(savedToken);
            if (jwtClaims is null)
            { // invalid token
                await MarkUserAsLoggedOut();
                return _anonymous;
            }
            var expiryClaimValue = jwtClaims.FirstOrDefault(c => c.Type == "exp").Value;
            if (DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryClaimValue)).DateTime <= DateTime.Now)
            { // token expired
                await MarkUserAsLoggedOut();
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(jwtClaims, "jwt")));
        }

        public async Task MarkUserAsAuthenticated(string token)
        {
            await _localStorage.SetItemAsync(jwtTokenKey, token);
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorage.RemoveItemAsync(jwtTokenKey);
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
