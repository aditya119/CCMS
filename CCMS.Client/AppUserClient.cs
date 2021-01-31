using CCMS.Shared.Models.AppUserModels;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace CCMS.Client
{
    public class AppUserClient
    {
        private readonly HttpClient _http;
        private readonly string baseUrl = "api/AppUser";

        public AppUserClient(HttpClient http)
        {
            _http = http;
        }

        public Task<IEnumerable<UserListItemModel>> GetAllUsersAsync()
        {
            return _http.GetFromJsonAsync<IEnumerable<UserListItemModel>>(baseUrl);
        }

        public Task<IEnumerable<UserListItemModel>> GetAllUsersWithRolesAsync(int roles)
        {
            return _http.GetFromJsonAsync<IEnumerable<UserListItemModel>>($"{baseUrl}/roles/{roles}");
        }

        public Task<UserDetailsModel> GetUserByIdAsync(int userId)
        {
            return _http.GetFromJsonAsync<UserDetailsModel>($"{baseUrl}/{userId}");
        }

        public Task<HttpResponseMessage> AddNewUserAsync(NewUserModel userDetails)
        {
            return _http.PostAsJsonAsync(baseUrl, userDetails);
        }

        public Task<HttpResponseMessage> UpdateUserAsync(UserDetailsModel userDetails)
        {
            return _http.PutAsJsonAsync(baseUrl, userDetails);
        }

        public Task<HttpResponseMessage> UpdatePasswordAsync(ChangePasswordModel changePasswordModel)
        {
            return _http.PutAsJsonAsync($"{baseUrl}/password", changePasswordModel);
        }

        public Task<HttpResponseMessage> ResetPasswordAsync(int userId)
        {
            return _http.PutAsync($"{baseUrl}/{userId}/password/reset", null);
        }

        public Task<HttpResponseMessage> UnlockAccountAsync(int userId)
        {
            return _http.PutAsync($"{baseUrl}/{userId}/unlock", null);
        }

        public Task<HttpResponseMessage> DeleteLawyerAsync(int userId)
        {
            return _http.DeleteAsync($"{baseUrl}/{userId}");
        }
    }
}
