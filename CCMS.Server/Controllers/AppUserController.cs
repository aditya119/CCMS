using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.AppUserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUsersService _usersService;
        private readonly ISessionService _sessionService;
        private readonly IAuthService _authService;
        private readonly ICryptoService _cryptoService;
        private readonly string defaultPassword = "manager";

        public AppUserController(IAppUsersService usersService,
            ISessionService sessionService,
            IAuthService authService,
            ICryptoService cryptoService)
        {
            _usersService = usersService;
            _sessionService = sessionService;
            _authService = authService;
            _cryptoService = cryptoService;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserListItemModel>>> GetAllUsers()
        {
            IEnumerable<UserListItemModel> allUsers = await _usersService.RetrieveAllAsync();
            return Ok(allUsers);
        }

        [HttpGet]
        [Route("roles/{roles:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Manager")]
        public async Task<ActionResult<IEnumerable<UserListItemModel>>> GetAllUsersWithRoles(int roles)
        {
            if (roles < 1)
            {
                return UnprocessableEntity($"Invalid Roles: {roles}");
            }
            IEnumerable<UserListItemModel> allUsers = await _usersService.RetrieveAllWithRolesAsync(roles);
            return Ok(allUsers);
        }

        [HttpGet]
        [Route("{userId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<UserDetailsModel>> GetUserDetails(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            bool hasAdminRole = _sessionService.IsInRoles(HttpContext, "Administrator");
            if (userId != currUser && hasAdminRole == false)
            {
                return Unauthorized();
            }
            UserDetailsModel userDetails = await _usersService.RetrieveAsync(userId);
            return Ok(userDetails);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateNewUser(NewUserModel userModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            string passwordSalt = _cryptoService.GenerateRandomSalt();
            string hashedPassword = _cryptoService.SaltAndHashText(defaultPassword, passwordSalt);
            int userId = await _usersService.CreateAsync(userModel, passwordSalt, hashedPassword);

            return Created("api/AppUser", userId);
        }

        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateUserDetails(UserDetailsModel userModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }

            await _usersService.UpdateAsync(userModel);
            return NoContent();
        }

        [HttpPut]
        [Route("password")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            if (changePasswordModel.UserId != currUser)
            {
                return Unauthorized();
            }
            string userEmail = _sessionService.GetUserEmail(HttpContext);
            (_, string hashedPassword, string salt) = await _authService.FetchUserDetailsAsync(userEmail);
            if (_cryptoService.SaltAndHashText(changePasswordModel.OldPassword, salt) != hashedPassword)
            {
                return UnprocessableEntity("Old Password Invalid");
            }
            string newPasswordHash = _cryptoService.SaltAndHashText(changePasswordModel.NewPassword, salt);

            await _usersService.ChangePasswordAsync(changePasswordModel.UserId, newPasswordHash);

            return NoContent();
        }

        [HttpPut]
        [Route("password/reset/{userId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            string userEmail = (await _usersService.RetrieveAsync(userId)).UserEmail;
            (_, _, string salt) = await _authService.FetchUserDetailsAsync(userEmail);
            string newPasswordHash = _cryptoService.SaltAndHashText(defaultPassword, salt);

            await _usersService.ChangePasswordAsync(userId, newPasswordHash);

            return NoContent();
        }

        [HttpPut]
        [Route("unlock/{userId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UnlockAccount(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }

            await _usersService.UnlockAccountAsync(userId);

            return NoContent();
        }

        [HttpDelete]
        [Route("{userId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            await _usersService.DeleteAsync(userId);
            return NoContent();
        }
    }
}
