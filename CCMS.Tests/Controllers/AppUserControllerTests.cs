using CCMS.Server.Controllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.AppUserModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers
{
    public class AppUserControllerTests
    {
        private readonly AppUserController _sut;
        private readonly IAppUsersService _mockAppUsersService = Substitute.For<IAppUsersService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        private readonly IAuthService _mockAuthService = Substitute.For<IAuthService>();
        private readonly ICryptoService _mockCryptoService = Substitute.For<ICryptoService>();
        private readonly string defaultPassword = "manager";
        public AppUserControllerTests()
        {
            _sut = new AppUserController(_mockAppUsersService, _mockSessionService, _mockAuthService, _mockCryptoService);
        }

        private static IEnumerable<UserListItemModel> GetSampleData_UserListItems()
        {
            var result = new List<UserListItemModel>
            {
                new UserListItemModel { UserId = 1, UserNameAndEmail = "ABC (abc@xyz.com)" },
                new UserListItemModel { UserId = 2, UserNameAndEmail = "DEF (def@xyz.com)" }
            };
            return result;
        }

        private static UserDetailsModel GetSampleData_UserDetails(int userId)
        {
            var result = new UserDetailsModel
            {
                UserId = userId,
                UserFullname = "Abc",
                UserEmail = "abc@xyz.com",
                UserRoles = 1
            };
            return result;
        }

        [Fact]
        public async Task GetAllUsers_Valid()
        {
            // Arrange
            IEnumerable<UserListItemModel> expected = GetSampleData_UserListItems();
            _mockAppUsersService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<UserListItemModel>> response = await _sut.GetAllUsers();

            // Assert
            await _mockAppUsersService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<UserListItemModel> actual = (IEnumerable<UserListItemModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).UserId, actual.ElementAt(i).UserId);
                Assert.Equal(expected.ElementAt(i).UserNameAndEmail, actual.ElementAt(i).UserNameAndEmail);
            }
        }

        [Fact]
        public async Task GetAllUsersWithRoles_UnprocessableEntity()
        {
            // Arrange
            int roles = 0;
            string expectedError = $"Invalid Roles: {roles}";

            // Act
            ActionResult<IEnumerable<UserListItemModel>> response = await _sut.GetAllUsersWithRoles(roles);

            // Assert
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().RetrieveAllWithRolesAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetAllUsersWithRoles_Valid()
        {
            // Arrange
            int roles = 1;
            IEnumerable<UserListItemModel> expected = GetSampleData_UserListItems();
            _mockAppUsersService.RetrieveAllWithRolesAsync(roles).Returns(expected);

            // Act
            ActionResult<IEnumerable<UserListItemModel>> response = await _sut.GetAllUsersWithRoles(roles);

            // Assert
            await _mockAppUsersService.Received(1).RetrieveAllWithRolesAsync(roles);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<UserListItemModel> actual = (IEnumerable<UserListItemModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).UserId, actual.ElementAt(i).UserId);
                Assert.Equal(expected.ElementAt(i).UserNameAndEmail, actual.ElementAt(i).UserNameAndEmail);
            }
        }

        [Fact]
        public async Task GetUserDetails_UnprocessableEntity()
        {
            // Arrange
            int userId = 0;
            string expectedError = $"Invalid UserId: {userId}";

            // Act
            ActionResult<UserDetailsModel> response = await _sut.GetUserDetails(userId);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().IsInRoles(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().RetrieveAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetUserDetails_Unauthorized()
        {
            // Arrange
            int userId = 1;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(userId + 1);
            _mockSessionService.IsInRoles(default, default).ReturnsForAnyArgs(false);

            // Act
            ActionResult<UserDetailsModel> response = await _sut.GetUserDetails(userId);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.ReceivedWithAnyArgs(1).IsInRoles(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().RetrieveAsync(default);
            Assert.IsType<UnauthorizedResult>(response.Result);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task GetUserDetails_NotFound(bool isCurrUser, bool isAdmin)
        {
            // Arrange
            int userId = 1;
            UserDetailsModel expected = null;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(isCurrUser ? userId : userId + 1);
            _mockSessionService.IsInRoles(default, default).ReturnsForAnyArgs(isAdmin);
            _mockAppUsersService.RetrieveAsync(userId).Returns(expected);

            // Act
            ActionResult<UserDetailsModel> response = await _sut.GetUserDetails(userId);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.ReceivedWithAnyArgs(1).IsInRoles(default, default);
            await _mockAppUsersService.Received(1).RetrieveAsync(userId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task GetUserDetails_Valid(bool isCurrUser, bool isAdmin)
        {
            // Arrange
            int userId = 1;
            UserDetailsModel expected = GetSampleData_UserDetails(userId);
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(isCurrUser ? userId : userId + 1);
            _mockSessionService.IsInRoles(default, default).ReturnsForAnyArgs(isAdmin);
            _mockAppUsersService.RetrieveAsync(userId).Returns(expected);

            // Act
            ActionResult<UserDetailsModel> response = await _sut.GetUserDetails(userId);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.ReceivedWithAnyArgs(1).IsInRoles(default, default);
            await _mockAppUsersService.Received(1).RetrieveAsync(userId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            UserDetailsModel actual = (UserDetailsModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.UserFullname, actual.UserFullname);
            Assert.Equal(expected.UserEmail, actual.UserEmail);
            Assert.Equal(expected.UserRoles, actual.UserRoles);
        }

        [Fact]
        public async Task CreateNewUser_ValidationProblem()
        {
            // Arrange
            NewUserModel userModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.CreateNewUser(userModel);

            // Assert
            _mockCryptoService.DidNotReceiveWithAnyArgs().GenerateRandomSalt();
            _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().CreateAsync(default, default, default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task CreateNewUser_Valid()
        {
            // Arrange
            NewUserModel userModel = new()
            {
                UserFullname = "Abc",
                UserEmail = "abc@xyz.com",
                UserRoles = 1
            };
            int userId = 1;
            string salt = "salt";
            string hash = "hash";
            _mockCryptoService.GenerateRandomSalt().Returns(salt);
            _mockCryptoService.SaltAndHashText(defaultPassword, salt).Returns(hash);
            _mockAppUsersService.CreateAsync(default, default, default).ReturnsForAnyArgs(userId);

            // Act
            IActionResult response = await _sut.CreateNewUser(userModel);

            // Assert
            _mockCryptoService.Received(1).GenerateRandomSalt();
            _mockCryptoService.Received(1).SaltAndHashText(defaultPassword, salt);
            await _mockAppUsersService.Received().CreateAsync(Arg.Is<NewUserModel>(p =>
            p.UserFullname == userModel.UserFullname && p.UserEmail == userModel.UserEmail && p.UserRoles == userModel.UserRoles),
                salt, hash);
            var createdAtActionResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(userId, (int)createdAtActionResult.Value);
            Assert.Equal("api/AppUser", createdAtActionResult.Location);
        }

        [Fact]
        public async Task UpdateUserDetails_ValidationProblem()
        {
            // Arrange
            UserDetailsModel userModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateUserDetails(userModel);

            // Assert
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().UpdateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateUserDetails_Valid()
        {
            // Arrange
            int updateAsyncCalled = 0;
            UserDetailsModel userModel = GetSampleData_UserDetails(1);
            _mockAppUsersService.WhenForAnyArgs(x => x.UpdateAsync(default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateUserDetails(userModel);

            // Assert
            await _mockAppUsersService.Received(1).UpdateAsync(Arg.Is<UserDetailsModel>(p =>
            p.UserId == userModel.UserId && p.UserFullname == userModel.UserFullname
            && p.UserEmail == userModel.UserEmail && p.UserRoles == userModel.UserRoles));
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task ChangePassword_ValidationProblem()
        {
            // Arrange
            ChangePasswordModel changePasswordModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.ChangePassword(changePasswordModel);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserEmail(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().FetchUserDetailsAsync(default);
            _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(default, defaultPassword);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task ChangePassword_Unauthorized()
        {
            // Arrange
            int userId = 1;
            ChangePasswordModel changePasswordModel = new()
            {
                UserId = userId,
                OldPassword = "old",
                NewPassword = "new",
                NewPasswordAgain = "new"
            };
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(userId + 1);

            // Act
            IActionResult response = await _sut.ChangePassword(changePasswordModel);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserEmail(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().FetchUserDetailsAsync(default);
            _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(default, defaultPassword);
            Assert.IsType<UnauthorizedResult>(response);
        }

        [Fact]
        public async Task ChangePassword_UnprocessableEntity()
        {
            // Arrange
            int userId = 1;
            string userEmail = "abc@xyz.com";
            string correctHash = "hash";
            string salt = "salt";
            string incorrectHash = "wrongHash";
            ChangePasswordModel changePasswordModel = new()
            {
                UserId = userId,
                OldPassword = "old",
                NewPassword = "new",
                NewPasswordAgain = "new"
            };
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(userId);
            _mockSessionService.GetUserEmail(default).ReturnsForAnyArgs(userEmail);
            _mockAuthService.FetchUserDetailsAsync(userEmail).Returns((userId, correctHash, salt));
            _mockCryptoService.SaltAndHashText(changePasswordModel.OldPassword, salt).Returns(incorrectHash);

            // Act
            IActionResult response = await _sut.ChangePassword(changePasswordModel);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserEmail(default);
            await _mockAuthService.Received(1).FetchUserDetailsAsync(userEmail);
            _mockCryptoService.Received(1).SaltAndHashText(changePasswordModel.OldPassword, salt);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal("Old Password Invalid", createdAtActionResult.Value);
        }

        [Fact]
        public async Task ChangePassword_Valid()
        {
            // Arrange
            int userId = 1;
            string userEmail = "abc@xyz.com";
            string hash = "hash";
            string salt = "salt";
            string newHash = "newHash";
            int changePasswordAsyncCalled = 0;
            ChangePasswordModel changePasswordModel = new()
            {
                UserId = userId,
                OldPassword = "old",
                NewPassword = "new",
                NewPasswordAgain = "new"
            };
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(userId);
            _mockSessionService.GetUserEmail(default).ReturnsForAnyArgs(userEmail);
            _mockAuthService.FetchUserDetailsAsync(userEmail).Returns((userId, hash, salt));
            _mockCryptoService.SaltAndHashText(changePasswordModel.OldPassword, salt).Returns(hash);
            _mockCryptoService.SaltAndHashText(changePasswordModel.NewPassword, salt).Returns(newHash);
            _mockAppUsersService.When(x => x.ChangePasswordAsync(userId, newHash))
                .Do(x => changePasswordAsyncCalled++);

            // Act
            IActionResult response = await _sut.ChangePassword(changePasswordModel);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserEmail(default);
            await _mockAuthService.Received(1).FetchUserDetailsAsync(userEmail);
            _mockCryptoService.Received().SaltAndHashText(changePasswordModel.OldPassword, salt);
            _mockCryptoService.Received().SaltAndHashText(changePasswordModel.NewPassword, salt);
            await _mockAppUsersService.Received(1).ChangePasswordAsync(userId, newHash);
            Assert.Equal(1, changePasswordAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task ResetPassword_UnprocessableEntity()
        {
            // Arrange
            int userId = 0;
            string expectedError = $"Invalid UserId: {userId}";

            // Act
            IActionResult response = await _sut.ResetPassword(userId);

            // Assert
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().RetrieveAsync(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().FetchUserDetailsAsync(default);
            _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task ResetPassword_NotFound()
        {
            // Arrange
            int userId = 1;
            UserDetailsModel expected = null;
            _mockAppUsersService.RetrieveAsync(userId).Returns(expected);

            // Act
            IActionResult response = await _sut.ResetPassword(userId);

            // Assert
            await _mockAppUsersService.Received(1).RetrieveAsync(userId);
            await _mockAuthService.DidNotReceiveWithAnyArgs().FetchUserDetailsAsync(default);
            _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().ChangePasswordAsync(default, default);
            Assert.IsType<NotFoundResult>(response);
        }

        [Fact]
        public async Task ResetPassword_Valid()
        {
            // Arrange
            int userId = 1;
            string hash = "hash";
            string salt = "salt";
            string newHash = "newHash";
            int changePasswordAsyncCalled = 0;
            UserDetailsModel expected = GetSampleData_UserDetails(userId);
            _mockAppUsersService.RetrieveAsync(userId).Returns(expected);
            _mockAuthService.FetchUserDetailsAsync(expected.UserEmail).Returns((userId, hash, salt));
            _mockCryptoService.SaltAndHashText(defaultPassword, salt).Returns(newHash);
            _mockAppUsersService.When(x => x.ChangePasswordAsync(userId, newHash))
                .Do(x => changePasswordAsyncCalled++);

            // Act
            IActionResult response = await _sut.ResetPassword(userId);

            // Assert
            await _mockAppUsersService.Received(1).RetrieveAsync(userId);
            await _mockAuthService.Received(1).FetchUserDetailsAsync(expected.UserEmail);
            _mockCryptoService.Received(1).SaltAndHashText(defaultPassword, salt);
            await _mockAppUsersService.Received(1).ChangePasswordAsync(userId, newHash);
            Assert.Equal(1, changePasswordAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task UnlockAccount_UnprocessableEntity()
        {
            // Arrange
            int userId = 0;
            string expectedError = $"Invalid UserId: {userId}";

            // Act
            IActionResult response = await _sut.UnlockAccount(userId);

            // Assert
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().UnlockAccountAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UnlockAccount_Valid()
        {
            // Arrange
            int userId = 1;
            int unlockAccountAsyncCalled = 0;
            _mockAppUsersService.When(x => x.UnlockAccountAsync(userId))
                .Do(x => unlockAccountAsyncCalled++);

            // Act
            IActionResult response = await _sut.UnlockAccount(userId);

            // Assert
            await _mockAppUsersService.Received(1).UnlockAccountAsync(userId);
            Assert.Equal(1, unlockAccountAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int userId = 0;
            string expectedError = $"Invalid UserId: {userId}";

            // Act
            IActionResult response = await _sut.Delete(userId);

            // Assert
            await _mockAppUsersService.DidNotReceiveWithAnyArgs().DeleteAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int userId = 1;
            int deleteAsyncCalled = 0;
            _mockAppUsersService.When(x => x.DeleteAsync(userId))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(userId);

            // Assert
            await _mockAppUsersService.Received(1).DeleteAsync(userId);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
