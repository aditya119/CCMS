using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.AppUserModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CCMS.Shared.Enums;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class AppUserController : ControllerBase
    {
        private readonly IAppUsersService _appUsersService;
        private readonly ISessionService _sessionService;
        private readonly IAuthService _authService;
        private readonly ICryptoService _cryptoService;
        private readonly string defaultPassword = "manager";

        public AppUserController(IAppUsersService appUsersService,
            ISessionService sessionService,
            IAuthService authService,
            ICryptoService cryptoService)
        {
            _appUsersService = appUsersService;
            _sessionService = sessionService;
            _authService = authService;
            _cryptoService = cryptoService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserListItemModel>>> GetAllUsers()
        {
            IEnumerable<UserListItemModel> allUsers = await _appUsersService.RetrieveAllAsync();
            return Ok(allUsers);
        }

        [HttpGet]
        [Route("roles/{roles:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Manager)]
        public async Task<ActionResult<IEnumerable<UserListItemModel>>> GetAllUsersWithRoles(int roles)
        {
            if (roles < 1)
            {
                return UnprocessableEntity($"Invalid Roles: {roles}");
            }
            IEnumerable<UserListItemModel> allUsers = await _appUsersService.RetrieveAllWithRolesAsync(roles);
            return Ok(allUsers);
        }

        [HttpGet]
        [Route("{userId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize]
        public async Task<ActionResult<UserDetailsModel>> GetUserDetails(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            bool hasAdminRole = _sessionService.IsInRoles(HttpContext, Roles.Administrator);
            if (userId != currUser && hasAdminRole == false)
            {
                return Forbid();
            }
            UserDetailsModel userDetails = await _appUsersService.RetrieveAsync(userId);
            if (userDetails is null)
            {
                return NotFound();
            }
            return Ok(userDetails);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateNewUser(NewUserModel userModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }
            string passwordSalt = _cryptoService.GenerateRandomSalt();
            string hashedPassword = _cryptoService.SaltAndHashText(defaultPassword, passwordSalt);
            int userId = await _appUsersService.CreateAsync(userModel, passwordSalt, hashedPassword);

            return Created("api/AppUser", userId);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UpdateUserDetails(UserDetailsModel userModel)
        {
            if (ModelState.IsValid == false)
            {
                return ValidationProblem();
            }

            await _appUsersService.UpdateAsync(userModel);
            return NoContent();
        }

        [HttpPut]
        [Route("password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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
                return Forbid();
            }
            string userEmail = _sessionService.GetUserEmail(HttpContext);
            (_, string hashedPassword, string salt) = await _authService.FetchUserDetailsAsync(userEmail);
            if (_cryptoService.SaltAndHashText(changePasswordModel.OldPassword, salt) != hashedPassword)
            {
                return UnprocessableEntity("Old Password Invalid");
            }
            string newPasswordHash = _cryptoService.SaltAndHashText(changePasswordModel.NewPassword, salt);

            await _appUsersService.ChangePasswordAsync(changePasswordModel.UserId, newPasswordHash);

            return NoContent();
        }

        [HttpPut]
        [Route("{userId:int}/password/reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            UserDetailsModel userDetails = await _appUsersService.RetrieveAsync(userId);
            if (userDetails is null)
            {
                return NotFound();
            }
            string userEmail = userDetails.UserEmail;
            (_, _, string salt) = await _authService.FetchUserDetailsAsync(userEmail);
            string newPasswordHash = _cryptoService.SaltAndHashText(defaultPassword, salt);

            await _appUsersService.ChangePasswordAsync(userId, newPasswordHash);

            return NoContent();
        }

        [HttpPut]
        [Route("{userId:int}/unlock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> UnlockAccount(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }

            await _appUsersService.UnlockAccountAsync(userId);

            return NoContent();
        }

        [HttpDelete]
        [Route("{userId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> Delete(int userId)
        {
            if (userId < 1)
            {
                return UnprocessableEntity($"Invalid UserId: {userId}");
            }
            await _appUsersService.DeleteAsync(userId);
            return NoContent();
        }
    }
}
