using CCMS.Server.Controllers;
using CCMS.Server.Services.DbServices;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using CCMS.Shared.Models;
using CCMS.Server.Models;
using CCMS.Server.Services;

namespace CCMS.Tests.Controllers
{
    public class LoginControllerTests
    {
        private readonly LoginController _sut;
        private readonly IAuthService _mockAuthService = Substitute.For<IAuthService>();
        private readonly ICryptoService _mockCryptoService = Substitute.For<ICryptoService>();
        public LoginControllerTests()
        {
            _sut = new LoginController(_mockAuthService, _mockCryptoService);
        }

        [Fact]
        public async Task Post_InvalidModelState()
        {
            // Arrange
            LoginModel loginModel = new ();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.Post(loginModel);

            // Assert
            _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().FetchUserDetailsAsync(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().LoginUserAsync(default);
            _mockCryptoService.DidNotReceiveWithAnyArgs().GenerateJSONWebToken(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Theory]
        [InlineData(-1, null, null, "Invalid Username or Password")]
        [InlineData(0, null, null, "Account Locked")]
        [InlineData(1, "HashOfCorrectPassword", "salt", "Invalid Username or Password")]
        public async Task Post_Unauthorized(int userId, string hashedPassword, string salt, string accountStatus)
        {
            // Arrange
            int incrementLoginCountCalled = 0;
            var loginModel = new LoginModel
            {
                UserEmail = "abc@xyz.com",
                Password = "IncorrectPassword",
                PlatformId = 1
            };
            _mockAuthService.FetchUserDetailsAsync(default).ReturnsForAnyArgs((userId, hashedPassword, salt));
            _mockCryptoService.SaltAndHashText(default, default).ReturnsForAnyArgs("HashOfIncorrectPassword");
            _mockAuthService.When(x => x.IncrementLoginCountAsync(userId))
                .Do(x => incrementLoginCountCalled++);

            // Act
            ActionResult<string> response = await _sut.Post(loginModel);

            // Assert
            await _mockAuthService.ReceivedWithAnyArgs(1).FetchUserDetailsAsync(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().LoginUserAsync(default);
            _mockCryptoService.DidNotReceiveWithAnyArgs().GenerateJSONWebToken(default);
            var createdAtActionResult = Assert.IsType<UnauthorizedObjectResult>(response.Result);
            Assert.Equal(accountStatus, createdAtActionResult.Value);
            if (userId == 1)
            {
                _mockCryptoService.Received(1).SaltAndHashText(Arg.Is(loginModel.Password), Arg.Is(salt));
                await _mockAuthService.ReceivedWithAnyArgs(1).IncrementLoginCountAsync(default);
                Assert.Equal(1, incrementLoginCountCalled);
            }
            else
            {
                _mockCryptoService.DidNotReceiveWithAnyArgs().SaltAndHashText(default, default);
            }
        }

        [Fact]
        public async Task Post_Valid()
        {
            // Arrange
            var loginModel = new LoginModel
            {
                UserEmail = "abc@xyz.com",
                Password = "CorrectPassword",
                PlatformId = 1
            };
            int userId = 1;
            string hashedPassword = "HashOfCorrectPassword";
            string salt = "salt";
            string rolesCsv = "Operator";
            string jwt = "JsonWebToken";
            _mockAuthService.FetchUserDetailsAsync(default).ReturnsForAnyArgs((userId, hashedPassword, salt));
            _mockCryptoService.SaltAndHashText(default, default).ReturnsForAnyArgs(hashedPassword);
            _mockAuthService.LoginUserAsync(default).ReturnsForAnyArgs(rolesCsv);
            _mockCryptoService.GenerateJSONWebToken(default).ReturnsForAnyArgs(jwt);

            // Act
            ActionResult<string> response = await _sut.Post(loginModel);

            // Assert
            await _mockAuthService.Received(1).FetchUserDetailsAsync(Arg.Is(loginModel.UserEmail));
            _mockCryptoService.Received(1).SaltAndHashText(Arg.Is(loginModel.Password), Arg.Is(salt));
            await _mockAuthService.DidNotReceiveWithAnyArgs().IncrementLoginCountAsync(default);
            await _mockAuthService.Received(1).LoginUserAsync(Arg.Is<SessionModel>
                (p => p.UserId == userId && p.PlatformId == loginModel.PlatformId));
            _mockCryptoService.Received(1).GenerateJSONWebToken(Arg.Is<JwtDetailsModel>
                (p => p.UserId == userId && p.PlatformId == loginModel.PlatformId && p.UserEmail == loginModel.UserEmail
                && string.Join(',', p.RolesArray) == rolesCsv));
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(jwt, createdAtActionResult.Value);
        }
    }
}
