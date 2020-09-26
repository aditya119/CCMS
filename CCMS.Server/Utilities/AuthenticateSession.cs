using CCMS.Server.DbServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace CCMS.Server.Utilities
{
    public class AuthenticateSession : AuthorizeAttribute, IAuthorizationFilter
    {
        private ISessionService _sessionService;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _sessionService = context.HttpContext.RequestServices.GetService(typeof(ISessionService)) as ISessionService;

            Task<bool> isValidSessionTask = _sessionService.IsValidSessionAsync(context.HttpContext);
            isValidSessionTask.Wait(1000);
            
            if (isValidSessionTask.Result == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
