using CCMS.Server.Controllers.ConfigurationControllers;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.LocationModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.ConfigurationControllers
{
    public class LocationControllerTests
    {
        private readonly LocationController _sut;
        private readonly ILocationsService _mockLocationsService = Substitute.For<ILocationsService>();
        public LocationControllerTests()
        {
            _sut = new LocationController(_mockLocationsService);
        }

        private static IEnumerable<LocationDetailsModel> GetSampleData()
        {
            var result = new List<LocationDetailsModel>
            {
                new LocationDetailsModel { LocationId = 1, LocationName = "Chandigarh" },
                new LocationDetailsModel { LocationId = 2, LocationName = "New Delhi" }
            };
            return result;
        }

        [Fact]
        public async Task GetAllLocations_Valid()
        {
            // Arrange
            IEnumerable<LocationDetailsModel> expected = GetSampleData();
            _mockLocationsService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<LocationDetailsModel>> response = await _sut.GetAllLocations();

            // Assert
            await _mockLocationsService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<LocationDetailsModel> actual = (IEnumerable<LocationDetailsModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).LocationId, actual.ElementAt(i).LocationId);
                Assert.Equal(expected.ElementAt(i).LocationName, actual.ElementAt(i).LocationName);
            }
        }

        [Fact]
        public async Task GetLocationDetails_UnprocessableEntity()
        {
            // Arrange
            int locationId = -1;
            string expectedError = $"Invalid LocationId: {locationId}";

            // Act
            ActionResult<LocationDetailsModel> response = await _sut.GetLocationDetails(locationId);

            // Assert
            await _mockLocationsService.DidNotReceiveWithAnyArgs().RetrieveAsync(locationId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetLocationDetails_NotFound()
        {
            // Arrange
            int locationId = 1;
            LocationDetailsModel expected = null;
            _mockLocationsService.RetrieveAsync(locationId).Returns(expected);

            // Act
            ActionResult<LocationDetailsModel> response = await _sut.GetLocationDetails(locationId);

            // Assert
            await _mockLocationsService.Received(1).RetrieveAsync(locationId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetLocationDetails_Valid()
        {
            // Arrange
            int locationId = 1;
            LocationDetailsModel expected = GetSampleData().FirstOrDefault(x => x.LocationId == locationId);
            _mockLocationsService.RetrieveAsync(locationId).Returns(expected);

            // Act
            ActionResult<LocationDetailsModel> response = await _sut.GetLocationDetails(locationId);

            // Assert
            await _mockLocationsService.Received(1).RetrieveAsync(locationId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            LocationDetailsModel actual = (LocationDetailsModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.LocationId, actual.LocationId);
            Assert.Equal(expected.LocationName, actual.LocationName);
        }

        [Fact]
        public async Task CreateNewLocation_ValidationProblem()
        {
            // Arrange
            NewLocationModel locationModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.CreateNewLocation(locationModel);

            // Assert
            await _mockLocationsService.DidNotReceiveWithAnyArgs().CreateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task CreateNewLocation_Valid()
        {
            // Arrange
            NewLocationModel locationModel = new()
            {
                LocationName = "Chandigarh"
            };
            int locationId = 1;
            _mockLocationsService.CreateAsync(default).ReturnsForAnyArgs(locationId);

            // Act
            IActionResult response = await _sut.CreateNewLocation(locationModel);

            // Assert
            await _mockLocationsService.Received().CreateAsync(Arg.Is<NewLocationModel>(p => p.LocationName == locationModel.LocationName));
            var createdAtActionResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(locationId, (int)createdAtActionResult.Value);
            Assert.Equal("api/config/Location", createdAtActionResult.Location);
        }

        [Fact]
        public async Task UpdateLocationDetails_ValidationProblem()
        {
            // Arrange
            LocationDetailsModel locationModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateLocationDetails(locationModel);

            // Assert
            await _mockLocationsService.DidNotReceiveWithAnyArgs().UpdateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateLocationDetails_Valid()
        {
            // Arrange
            int updateAsyncCalled = 0;
            LocationDetailsModel locationModel = new()
            {
                LocationId = 1,
                LocationName = "Chandigarh"
            };
            _mockLocationsService.WhenForAnyArgs(x => x.UpdateAsync(default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateLocationDetails(locationModel);

            // Assert
            await _mockLocationsService.Received().UpdateAsync(Arg.Is<LocationDetailsModel>(p =>
            p.LocationName == locationModel.LocationName && p.LocationId == locationModel.LocationId));
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int locationId = 0;
            string expectedError = $"Invalid LocationId: {locationId}";

            // Act
            IActionResult response = await _sut.Delete(locationId);

            // Assert
            await _mockLocationsService.DidNotReceiveWithAnyArgs().DeleteAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int locationId = 1;
            int deleteAsyncCalled = 0;
            _mockLocationsService.WhenForAnyArgs(x => x.DeleteAsync(locationId))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(locationId);

            // Assert
            await _mockLocationsService.Received(1).DeleteAsync(locationId);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
