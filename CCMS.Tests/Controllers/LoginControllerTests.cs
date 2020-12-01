using CCMS.Server.Controllers;
using CCMS.Server.Services.DbServices;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using CCMS.Shared.Models;

namespace CCMS.Tests.Controllers
{
    public class LoginControllerTests
    {
        private readonly LoginController _sut;
        private readonly IAuthService _mockAuthService = Substitute.For<IAuthService>();
        private readonly IConfiguration _mockConfiguration = Substitute.For<IConfiguration>();
        public LoginControllerTests()
        {
            _sut = new LoginController(_mockAuthService, _mockConfiguration);
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
            await _mockAuthService.DidNotReceiveWithAnyArgs().FetchUserDetailsAsync(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().LoginUserAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Theory]
        [InlineData(-1, null, null, "Invalid Username or Password")]
        [InlineData(0, null, null, "Account Locked")]
        [InlineData(1, "hash", "salt", "Invalid Username or Password")]
        public async Task Post_Unauthorized(int userId, string hashedPassword, string salt, string accountStatus)
        {
            // Arrange
            int incrementLoginCountCalled = 0;
            LoginModel loginModel = new();
            _mockAuthService.FetchUserDetailsAsync(default).ReturnsForAnyArgs((userId, hashedPassword, salt));
            _mockAuthService.When(x => x.IncrementLoginCountAsync(userId))
                .Do(x => incrementLoginCountCalled++);

            // Act
            ActionResult<string> response = await _sut.Post(loginModel);

            // Assert
            await _mockAuthService.ReceivedWithAnyArgs(1).FetchUserDetailsAsync(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().LoginUserAsync(default);
            var createdAtActionResult = Assert.IsType<UnauthorizedObjectResult>(response.Result);
            Assert.Equal(accountStatus, createdAtActionResult.Value);
            if (userId == 1)
            {
                await _mockAuthService.ReceivedWithAnyArgs(1).IncrementLoginCountAsync(default);
                Assert.Equal(1, incrementLoginCountCalled);
            }
        }
    }
}
