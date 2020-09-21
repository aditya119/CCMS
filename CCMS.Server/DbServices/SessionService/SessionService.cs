using CCMS.Server.DbDataAccess;
using CCMS.Server.Utilities;
using CCMS.Shared.Models;
using Dapper.Oracle;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class SessionService : ISessionService
    {
        private readonly IOracleDataAccess _dataAccess;

        public SessionService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }
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
        private string GetGuid(HttpContext httpContext)
        {
            string guid = GetClaimFromToken(httpContext, JwtRegisteredClaimNames.Jti);
            return guid;
        }
        private SessionModel GenerateSessionModelFromHttpContext(HttpContext httpContext)
        {
            return new SessionModel
            {
                UserId = GetUserId(httpContext),
                PlatformId = GetPlatformId(httpContext),
                Guid = GetGuid(httpContext)
            };
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
        private async Task<bool> ExistsSessionInDb(SessionModel sessionModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_user_id", sessionModel.UserId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_platform_id", sessionModel.PlatformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            parameters.Add("pi_guid", sessionModel.Guid, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            parameters.Add("po_is_valid", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            await _dataAccess.ExecuteAsync("pkg_auth.p_is_valid_session", parameters);

            return (int)parameters.Get<decimal>("po_is_valid") == 1;
        }
        public async Task<bool> IsValidSessionAsync(HttpContext httpContext)
        {
            SessionModel sessionModel = GenerateSessionModelFromHttpContext(httpContext);
            if (SessionCacheUtil.ExistsSessionInCache(sessionModel))
            {
                return true;
            }
            return await ExistsSessionInDb(sessionModel);
        }
        private async Task ClearSessionFromDb(SessionModel sessionModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_user_id", sessionModel.UserId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_platform_id", sessionModel.PlatformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            await _dataAccess.ExecuteAsync("pkg_auth.p_logout", parameters);
        }
        public async Task ClearSessionAsync(HttpContext httpContext)
        {
            SessionModel sessionModel = GenerateSessionModelFromHttpContext(httpContext);
            SessionCacheUtil.ClearSessionFromCache(sessionModel);
            await ClearSessionFromDb(sessionModel);
        }
    }
}
