using CCMS.Server.DbServices;
using CCMS.Server.Services;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace CCMS.Server.Utilities
{
    public class AuthenticateSession : AuthorizeAttribute, IAuthorizationFilter
    {
        private ISessionService _sessionService;
        private IAuthService _authService;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _sessionService = context.HttpContext.RequestServices.GetService(typeof(ISessionService)) as ISessionService;
            _authService = context.HttpContext.RequestServices.GetService(typeof(IAuthService)) as IAuthService;
            SessionModel sessionModel = _sessionService.GenerateSessionModelFromHttpContext(context.HttpContext);
            Task<bool> isValidSessionTask = _authService.IsValidSessionAsync(sessionModel);
            isValidSessionTask.Wait(1000);
            
            if (isValidSessionTask.Result == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
