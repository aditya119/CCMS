using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface ISessionService
    {
        Task ClearSessionAsync(HttpContext httpContext);
        int GetPlatformId(HttpContext httpContext);
        IEnumerable<string> GetRoles(HttpContext httpContext);
        string GetUserEmail(HttpContext httpContext);
        int GetUserId(HttpContext httpContext);
        bool IsInRoles(HttpContext httpContext, string rolesCsv);
        Task<bool> IsValidSessionAsync(HttpContext httpContext);
    }
}