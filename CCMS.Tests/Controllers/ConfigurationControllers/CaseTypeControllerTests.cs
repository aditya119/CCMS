using CCMS.Server.Controllers.ConfigurationControllers;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.CaseTypeModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.ConfigurationControllers
{
    public class CaseTypeControllerTests
    {
        private readonly CaseTypeController _sut;
        private readonly ICaseTypesService _mockCaseTypesService = Substitute.For<ICaseTypesService>();
        public CaseTypeControllerTests()
        {
            _sut = new CaseTypeController(_mockCaseTypesService);
        }

        private static IEnumerable<CaseTypeDetailsModel> GetSampleData()
        {
            var result = new List<CaseTypeDetailsModel>
            {
                new CaseTypeDetailsModel { CaseTypeId = 1, CaseTypeName = "Civil" },
                new CaseTypeDetailsModel { CaseTypeId = 2, CaseTypeName = "Criminal" }
            };
            return result;
        }

        [Fact]
        public async Task GetAllCaseTypes_Valid()
        {
            // Arrange
            IEnumerable<CaseTypeDetailsModel> expected = GetSampleData();
            _mockCaseTypesService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<CaseTypeDetailsModel>> response = await _sut.GetAllCaseTypes();

            // Assert
            await _mockCaseTypesService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<CaseTypeDetailsModel> actual = (IEnumerable<CaseTypeDetailsModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseTypeId, actual.ElementAt(i).CaseTypeId);
                Assert.Equal(expected.ElementAt(i).CaseTypeName, actual.ElementAt(i).CaseTypeName);
            }
        }

        [Fact]
        public async Task GetCaseTypeDetails_UnprocessableEntity()
        {
            // Arrange
            int caseTypeId = -1;
            string expectedError = $"Invalid CaseTypeId: {caseTypeId}";

            // Act
            ActionResult<CaseTypeDetailsModel> response = await _sut.GetCaseTypeDetails(caseTypeId);

            // Assert
            await _mockCaseTypesService.DidNotReceiveWithAnyArgs().RetrieveAsync(caseTypeId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCaseTypeDetails_NotFound()
        {
            // Arrange
            int caseTypeId = 1;
            CaseTypeDetailsModel expected = null;
            _mockCaseTypesService.RetrieveAsync(caseTypeId).Returns(expected);

            // Act
            ActionResult<CaseTypeDetailsModel> response = await _sut.GetCaseTypeDetails(caseTypeId);

            // Assert
            await _mockCaseTypesService.Received(1).RetrieveAsync(caseTypeId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCaseTypeDetails_Valid()
        {
            // Arrange
            int caseTypeId = 1;
            CaseTypeDetailsModel expected = GetSampleData().FirstOrDefault(x => x.CaseTypeId == caseTypeId);
            _mockCaseTypesService.RetrieveAsync(caseTypeId).Returns(expected);

            // Act
            ActionResult<CaseTypeDetailsModel> response = await _sut.GetCaseTypeDetails(caseTypeId);

            // Assert
            await _mockCaseTypesService.Received(1).RetrieveAsync(caseTypeId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            CaseTypeDetailsModel actual = (CaseTypeDetailsModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseTypeId, actual.CaseTypeId);
            Assert.Equal(expected.CaseTypeName, actual.CaseTypeName);
        }

        [Fact]
        public async Task CreateNewCaseType_ValidationProblem()
        {
            // Arrange
            NewCaseTypeModel caseTypeModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.CreateNewCaseType(caseTypeModel);

            // Assert
            await _mockCaseTypesService.DidNotReceiveWithAnyArgs().CreateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task CreateNewCaseType_Valid()
        {
            // Arrange
            NewCaseTypeModel caseTypeModel = new()
            { 
                CaseTypeName = "Criminal"
            };
            int caseTypeId = 1;
            _mockCaseTypesService.CreateAsync(default).ReturnsForAnyArgs(caseTypeId);

            // Act
            IActionResult response = await _sut.CreateNewCaseType(caseTypeModel);

            // Assert
            await _mockCaseTypesService.Received().CreateAsync(Arg.Is<NewCaseTypeModel>(p => p.CaseTypeName == caseTypeModel.CaseTypeName));
            var createdAtActionResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(caseTypeId, (int)createdAtActionResult.Value);
            Assert.Equal("api/config/CaseType", createdAtActionResult.Location);
        }

        [Fact]
        public async Task UpdateCaseTypeDetails_ValidationProblem()
        {
            // Arrange
            CaseTypeDetailsModel caseTypeModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateCaseTypeDetails(caseTypeModel);

            // Assert
            await _mockCaseTypesService.DidNotReceiveWithAnyArgs().UpdateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateCaseTypeDetails_Valid()
        {
            // Arrange
            int updateAsyncCalled = 0;
            CaseTypeDetailsModel caseTypeModel = new()
            {
                CaseTypeId = 1,
                CaseTypeName = "Criminal"
            };
            _mockCaseTypesService.WhenForAnyArgs(x => x.UpdateAsync(default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateCaseTypeDetails(caseTypeModel);

            // Assert
            await _mockCaseTypesService.Received().UpdateAsync(Arg.Is<CaseTypeDetailsModel>(p =>
            p.CaseTypeName == caseTypeModel.CaseTypeName && p.CaseTypeId == caseTypeModel.CaseTypeId));
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int caseTypeId = 0;
            string expectedError = $"Invalid CaseTypeId: {caseTypeId}";

            // Act
            IActionResult response = await _sut.Delete(caseTypeId);

            // Assert
            await _mockCaseTypesService.DidNotReceiveWithAnyArgs().DeleteAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int caseTypeId = 1;
            int deleteAsyncCalled = 0;
            _mockCaseTypesService.WhenForAnyArgs(x => x.DeleteAsync(caseTypeId))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(caseTypeId);

            // Assert
            await _mockCaseTypesService.Received(1).DeleteAsync(caseTypeId);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
