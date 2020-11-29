using CCMS.Shared.Models.AppUserModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IAppUsersService
    {
        Task ChangePasswordAsync(int userId, string userPassword);
        Task<int> CreateAsync(NewUserModel userModel, string passwordSalt, string hashedPassword);
        Task DeleteAsync(int userId);
        Task<IEnumerable<UserListItemModel>> RetrieveAllAsync();
        Task<IEnumerable<UserListItemModel>> RetrieveAllWithRolesAsync(int roles);
        Task<UserDetailsModel> RetrieveAsync(int userId);
        Task UpdateAsync(UserDetailsModel userModel);
    }
}