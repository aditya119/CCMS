using System;
using System.Threading.Tasks;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICryptoService _cryptoService;

        public LoginController(IAuthService authService, ICryptoService cryptoService)
        {
            _authService = authService;
            _cryptoService = cryptoService;
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
            if (_cryptoService.SaltAndHashText(loginModel.Password, salt) != hashedPassword)
            {
                await _authService.IncrementLoginCountAsync(userId);
                return Unauthorized(accountStatus);
            }
            string guid = Guid.NewGuid().ToString();

            SessionModel sessionModel = new(userId, loginModel.PlatformId, guid);
            string userRolesCsv = await _authService.LoginUserAsync(sessionModel);

            JwtDetailsModel jwtDetailsModel = new(userId, loginModel.UserEmail, loginModel.PlatformId, userRolesCsv, guid);
            string token = _cryptoService.GenerateJSONWebToken(jwtDetailsModel);
            
            return Ok(token);
        }
    }
}
