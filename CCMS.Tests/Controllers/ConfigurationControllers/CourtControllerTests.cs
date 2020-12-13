using CCMS.Server.Controllers.ConfigurationControllers;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.CourtModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.ConfigurationControllers
{
    public class CourtControllerTests
    {
        private readonly CourtController _sut;
        private readonly ICourtsService _mockCourtsService = Substitute.For<ICourtsService>();
        public CourtControllerTests()
        {
            _sut = new CourtController(_mockCourtsService);
        }

        private static IEnumerable<CourtDetailsModel> GetSampleData()
        {
            var result = new List<CourtDetailsModel>
            {
                new CourtDetailsModel { CourtId = 1, CourtName = "Supreme Court" },
                new CourtDetailsModel { CourtId = 2, CourtName = "High Court" }
            };
            return result;
        }

        [Fact]
        public async Task GetAllCourts_Valid()
        {
            // Arrange
            IEnumerable<CourtDetailsModel> expected = GetSampleData();
            _mockCourtsService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<CourtDetailsModel>> response = await _sut.GetAllCourts();

            // Assert
            await _mockCourtsService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<CourtDetailsModel> actual = (IEnumerable<CourtDetailsModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CourtId, actual.ElementAt(i).CourtId);
                Assert.Equal(expected.ElementAt(i).CourtName, actual.ElementAt(i).CourtName);
            }
        }

        [Fact]
        public async Task GetCourtDetails_UnprocessableEntity()
        {
            // Arrange
            int courtId = -1;
            string expectedError = $"Invalid CourtId: {courtId}";

            // Act
            ActionResult<CourtDetailsModel> response = await _sut.GetCourtDetails(courtId);

            // Assert
            await _mockCourtsService.DidNotReceiveWithAnyArgs().RetrieveAsync(courtId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCourtDetails_NotFound()
        {
            // Arrange
            int courtId = 1;
            CourtDetailsModel expected = null;
            _mockCourtsService.RetrieveAsync(courtId).Returns(expected);

            // Act
            ActionResult<CourtDetailsModel> response = await _sut.GetCourtDetails(courtId);

            // Assert
            await _mockCourtsService.Received(1).RetrieveAsync(courtId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCourtDetails_Valid()
        {
            // Arrange
            int courtId = 1;
            CourtDetailsModel expected = GetSampleData().FirstOrDefault(x => x.CourtId == courtId);
            _mockCourtsService.RetrieveAsync(courtId).Returns(expected);

            // Act
            ActionResult<CourtDetailsModel> response = await _sut.GetCourtDetails(courtId);

            // Assert
            await _mockCourtsService.Received(1).RetrieveAsync(courtId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            CourtDetailsModel actual = (CourtDetailsModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.CourtId, actual.CourtId);
            Assert.Equal(expected.CourtName, actual.CourtName);
        }

        [Fact]
        public async Task CreateNewCourt_ValidationProblem()
        {
            // Arrange
            NewCourtModel courtModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.CreateNewCourt(courtModel);

            // Assert
            await _mockCourtsService.DidNotReceiveWithAnyArgs().CreateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task CreateNewCourt_Valid()
        {
            // Arrange
            NewCourtModel courtModel = new()
            {
                CourtName = "Supreme Court"
            };
            int courtId = 1;
            _mockCourtsService.CreateAsync(default).ReturnsForAnyArgs(courtId);

            // Act
            IActionResult response = await _sut.CreateNewCourt(courtModel);

            // Assert
            await _mockCourtsService.Received().CreateAsync(Arg.Is<NewCourtModel>(p => p.CourtName == courtModel.CourtName));
            var createdAtActionResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(courtId, (int)createdAtActionResult.Value);
            Assert.Equal("api/config/Court", createdAtActionResult.Location);
        }

        [Fact]
        public async Task UpdateCourtDetails_ValidationProblem()
        {
            // Arrange
            CourtDetailsModel courtModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateCourtDetails(courtModel);

            // Assert
            await _mockCourtsService.DidNotReceiveWithAnyArgs().UpdateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateCourtDetails_Valid()
        {
            // Arrange
            int updateAsyncCalled = 0;
            CourtDetailsModel courtModel = new()
            {
                CourtId = 1,
                CourtName = "Supreme Court"
            };
            _mockCourtsService.WhenForAnyArgs(x => x.UpdateAsync(default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateCourtDetails(courtModel);

            // Assert
            await _mockCourtsService.Received().UpdateAsync(Arg.Is<CourtDetailsModel>(p =>
            p.CourtName == courtModel.CourtName && p.CourtId == courtModel.CourtId));
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int courtId = 0;
            string expectedError = $"Invalid CourtId: {courtId}";

            // Act
            IActionResult response = await _sut.Delete(courtId);

            // Assert
            await _mockCourtsService.DidNotReceiveWithAnyArgs().DeleteAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int courtId = 1;
            int deleteAsyncCalled = 0;
            _mockCourtsService.WhenForAnyArgs(x => x.DeleteAsync(courtId))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(courtId);

            // Assert
            await _mockCourtsService.Received(1).DeleteAsync(courtId);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
