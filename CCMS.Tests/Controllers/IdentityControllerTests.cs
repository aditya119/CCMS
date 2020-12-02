using System.Collections.Generic;
using CCMS.Server.Controllers;
using CCMS.Server.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace CCMS.Tests.Controllers
{
    public class IdentityControllerTests
    {
        private readonly IdentityController _sut;
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public IdentityControllerTests()
        {
            _sut = new IdentityController(_mockSessionService);
        }

        [Fact]
        public void GetUserId_Valid()
        {
            // Arrange
            int userId = 1;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(userId);

            // Act
            ActionResult<int> response = _sut.GetUserId();

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(userId, createdAtActionResult.Value);
        }

        [Fact]
        public void GetUserEmail_Valid()
        {
            // Arrange
            string userEmail = "abc@xyz.com";
            _mockSessionService.GetUserEmail(default).ReturnsForAnyArgs(userEmail);

            // Act
            ActionResult<string> response = _sut.GetUserEmail();

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserEmail(default);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(userEmail, createdAtActionResult.Value);
        }

        [Fact]
        public void GetRoles_Valid()
        {
            // Arrange
            IEnumerable<string> roles = new List<string>
            { "Operator", "Manager" };
            _mockSessionService.GetRoles(default).ReturnsForAnyArgs(roles);

            // Act
            ActionResult<string> response = _sut.GetRoles();

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetRoles(default);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(string.Join(',', roles), createdAtActionResult.Value);
        }

        [Fact]
        public void GetPlatformId_Valid()
        {
            // Arrange
            int platformId = 1;
            _mockSessionService.GetPlatformId(default).ReturnsForAnyArgs(platformId);

            // Act
            ActionResult<int> response = _sut.GetPlatformId();

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetPlatformId(default);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            Assert.Equal(platformId, createdAtActionResult.Value);
        }
    }
}
