using CCMS.Server.Models;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IAuthService
    {
        Task<(int, string, string)> FetchUserDetailsAsync(string userEmail);
        Task IncrementLoginCountAsync(int userId);
        Task<bool> IsValidSessionAsync(SessionModel sessionModel);
        Task<string> LoginUserAsync(SessionModel sessionModel);
        Task LogoutAsync(int userId, int platformId);
    }
}