using CCMS.Server.Controllers;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Tests.Controllers
{
    public class LogoutControllerTests
    {
        private readonly LogoutController _sut;
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        private readonly IAuthService _mockAuthService = Substitute.For<IAuthService>();
        public LogoutControllerTests()
        {
            _sut = new LogoutController(_mockSessionService, _mockAuthService);
        }

        [Fact]
        public async Task Post_Valid()
        {
            // Arrange
            int userId = 1;
            int platformId = 1;
            int logoutCounter = 0;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(userId);
            _mockSessionService.GetPlatformId(default).ReturnsForAnyArgs(platformId);
            _mockAuthService.When(x => x.LogoutAsync(userId, platformId))
                .Do(x => logoutCounter++);

            // Act
            IActionResult response = await _sut.Post();

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            _mockSessionService.ReceivedWithAnyArgs(1).GetPlatformId(default);
            await _mockAuthService.Received(1).LogoutAsync(Arg.Is<int>(p => p == userId), Arg.Is<int>(q => q == platformId));
            Assert.Equal(1, logoutCounter);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
