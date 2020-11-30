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
        [InlineData(0, null, null)]
        [InlineData(1, "hash", "salt")]
        public async Task Post_Unauthorized(int userId, string hashedPassword, string salt)
        {
            // Arrange
            LoginModel loginModel = new();
            _mockAuthService.FetchUserDetailsAsync(default).ReturnsForAnyArgs((userId, hashedPassword, salt));

            // Act
            var response = await _sut.Post(loginModel);

            // Assert
            await _mockAuthService.ReceivedWithAnyArgs(1).FetchUserDetailsAsync(default);
            await _mockAuthService.DidNotReceiveWithAnyArgs().LoginUserAsync(default);
            Assert.IsType<UnauthorizedResult>(response.Result);
        }
    }
}
