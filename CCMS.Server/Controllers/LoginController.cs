using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models;
using Dapper.Oracle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IOracleDataAccess _oracleDataAccess;

        public LoginController(IOracleDataAccess oracleDataAccess)
        {
            _oracleDataAccess = oracleDataAccess;
        }
        private async Task<(int, string, string)> FetchUserDetails(LoginModel loginModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_user_email", loginModel.UserEmail, OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_user_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            parameters.Add("po_password", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output);
            parameters.Add("po_salt", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output);
            await _oracleDataAccess.ExecuteAsync("pkg_auth.p_get_auth_details", parameters);

            int? userId = parameters.Get<int?>("po_user_id");
            string password = parameters.Get<string>("po_password");
            string salt = parameters.Get<string>("po_salt");

            if (userId.HasValue)
            {
                return (userId.Value, password, salt);
            }
            return (0, string.Empty, string.Empty);
        }
        private async Task<bool> AuthenticateUser(LoginModel loginModel)
        {
            (int userId, string password, string salt) = await FetchUserDetails(loginModel);
            if (userId == 0)
            {
                return false;
            }
            if (loginModel.Password + salt == password)
            {
                return true;
            }
            return false;
        }
        /*[HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<string>> Post(LoginModel loginModel)
        {
            ActionResult response = Unauthorized();
            bool result = await AuthenticateUser(loginModel);
            if (result)
            {

            }
            if (result == 0)
            {
                var tokenString = GenerateJSONWebToken(userId, roles);
                response = Ok(tokenString);
            }
            return response;
        }

        private string GenerateJSONWebToken(int userId, int roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigUtil.JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer: ConfigUtil.JwtIssuer,
                audience: ConfigUtil.JwtAudience,
                claims: new[] {
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Azp, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Prn, roles.ToString())
                },
                expires: DateTime.Now.AddDays(ConfigUtil.JwtExpiryInDays),
                notBefore: DateTime.Now,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/
    }
}
