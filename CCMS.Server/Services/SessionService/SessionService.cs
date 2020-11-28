using CCMS.Shared.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace CCMS.Server.Services
{
    public class SessionService : ISessionService
    {
        private static ClaimsIdentity GetIdentityFromToken(HttpContext httpContext)
        {
            var identity = httpContext.User.Identity as ClaimsIdentity;
            return identity;
        }
        private static string GetClaimFromToken(HttpContext httpContext, string propertyName)
        {
            ClaimsIdentity identity = GetIdentityFromToken(httpContext);
            string value = identity.FindFirst(propertyName).Value;
            return value;
        }
        private static string GetGuid(HttpContext httpContext)
        {
            string guid = GetClaimFromToken(httpContext, JwtRegisteredClaimNames.Jti);
            return guid;
        }
        public SessionModel GenerateSessionModelFromHttpContext(HttpContext httpContext)
        {
            return new SessionModel(GetUserId(httpContext), GetPlatformId(httpContext), GetGuid(httpContext));
        }
        public int GetUserId(HttpContext httpContext)
        {
            int userId = int.Parse(GetClaimFromToken(httpContext, ClaimTypes.NameIdentifier));
            return userId;
        }
        public string GetUserEmail(HttpContext httpContext)
        {
            string userEmail = GetClaimFromToken(httpContext, ClaimTypes.Email);
            return userEmail;
        }
        public int GetPlatformId(HttpContext httpContext)
        {
            int platformId = int.Parse(GetClaimFromToken(httpContext, "platformId"));
            return platformId;
        }
        public IEnumerable<string> GetRoles(HttpContext httpContext)
        {
            ClaimsIdentity identity = GetIdentityFromToken(httpContext);
            IEnumerable<string> roles = identity.FindAll(ClaimTypes.Role).Select(p => p.Value.ToString());
            return roles;
        }
        public bool IsInRoles(HttpContext httpContext, string rolesCsv)
        {
            IEnumerable<string> userRoles = GetRoles(httpContext);
            IEnumerable<string> claimRoles = rolesCsv.Split(',');
            foreach (var role in claimRoles)
            {
                if (userRoles.Contains(role) == false)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
