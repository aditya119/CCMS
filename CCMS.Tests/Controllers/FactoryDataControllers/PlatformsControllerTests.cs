using CCMS.Server.Controllers.FactoryDataControllers;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.FactoryDataControllers
{
    public class PlatformsControllerTests
    {
        private readonly PlatformsController _sut;
        private readonly IPlatformsService _mockPlatformsService = Substitute.For<IPlatformsService>();
        public PlatformsControllerTests()
        {
            _sut = new PlatformsController(_mockPlatformsService);
        }

        private static IEnumerable<PlatformModel> GetSampleData()
        {
            var result = new List<PlatformModel>
            {
                new PlatformModel { PlatformId = 1, PlatformName = "MOBILE" },
                new PlatformModel { PlatformId = 2, PlatformName = "DESKTOP" }
            };
            return result;
        }

        [Fact]
        public async Task GetAllPlatforms_Valid()
        {
            // Arrange
            IEnumerable<PlatformModel> expected = GetSampleData();
            _mockPlatformsService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<PlatformModel>> response = await _sut.GetAllPlatforms();

            // Assert
            await _mockPlatformsService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<PlatformModel> actual = (IEnumerable<PlatformModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).PlatformId, actual.ElementAt(i).PlatformId);
                Assert.Equal(expected.ElementAt(i).PlatformName, actual.ElementAt(i).PlatformName);
            }
        }
    }
}
