using CCMS.Shared.Models;
using System.Threading.Tasks;

namespace CCMS.Web.Authenitication
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginModel loginModel);
        Task LogoutAsync();
    }
}