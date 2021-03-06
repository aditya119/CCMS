﻿using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CCMS.Server.Utilities
{
    public class AuthenticateSession : AuthorizeAttribute, IAuthorizationFilter
    {
        private ISessionService _sessionService;
        private IAuthService _authService;
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            _sessionService = context.HttpContext.RequestServices.GetRequiredService(typeof(ISessionService)) as ISessionService;
            _authService = context.HttpContext.RequestServices.GetRequiredService(typeof(IAuthService)) as IAuthService;

            SessionModel sessionModel = _sessionService.GenerateSessionModelFromHttpContext(context.HttpContext);
            var isValidSessionTask = _authService.IsValidSessionAsync(sessionModel);
            isValidSessionTask.Wait(5000);

            if (isValidSessionTask.Result == false)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }
    }
}
