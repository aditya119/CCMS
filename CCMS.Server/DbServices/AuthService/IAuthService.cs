using CCMS.Shared.Models;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IAuthService
    {
        Task<(int, string, string)> FetchUserDetailsAsync(string userEmail);
        Task<string> LoginUserAsync(int userId, int platformId, string guid);
    }
}