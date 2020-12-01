using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        public LoginController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        /// <summary>
        /// Validate login credentials
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns>Json-web token if credentials are valid</returns>
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<ActionResult<string>> Post(LoginModel loginModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            (int userId, string hashedPassword, string salt) = await _authService.FetchUserDetailsAsync(loginModel.UserEmail);
            string accountStatus = userId == 0 ? "Account Locked" : "Invalid Username or Password";
            if (userId < 1)
            {
                return Unauthorized(accountStatus);
            }
            if (HashUtil.VerifyPassword(hashedPassword, loginModel.Password, salt) == false)
            {
                await _authService.IncrementLoginCountAsync(userId);
                return Unauthorized(accountStatus);
            }
            string guid = Guid.NewGuid().ToString();

            SessionModel sessionModel = new (userId, loginModel.PlatformId, guid);
            string userRolesCsv = await _authService.LoginUserAsync(sessionModel);
                        
            string token = GenerateJSONWebToken(loginModel, userId, userRolesCsv, guid);
            
            return Ok(token);
        }

        private string GenerateJSONWebToken(LoginModel loginModel, int userId, string rolesCsv, string guid)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, guid),
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, loginModel.UserEmail),
                new Claim("platformId", loginModel.PlatformId.ToString())
            };
            var rolesAsStringArray = rolesCsv.ToString().Split(',');
            foreach (var stringRole in rolesAsStringArray)
            {
                claims.Add(new Claim(ClaimTypes.Role, stringRole.Trim()));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(int.Parse(_config["Jwt:ExpiryInDays"])),
                notBefore: DateTime.Now,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
