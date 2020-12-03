using CCMS.Server.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace CCMS.Server.Services
{
    public interface ISessionService
    {
        SessionModel GenerateSessionModelFromHttpContext(HttpContext httpContext);
        int GetPlatformId(HttpContext httpContext);
        IEnumerable<string> GetRoles(HttpContext httpContext);
        string GetUserEmail(HttpContext httpContext);
        int GetUserId(HttpContext httpContext);
        bool IsInRoles(HttpContext httpContext, string rolesCsv);
    }
}