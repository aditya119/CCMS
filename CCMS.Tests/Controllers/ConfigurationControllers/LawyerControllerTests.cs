using CCMS.Server.Controllers.ConfigurationControllers;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.LawyerModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.ConfigurationControllers
{
    public class LawyerControllerTests
    {
        private readonly LawyerController _sut;
        private readonly ILawyersService _mockLawyersService = Substitute.For<ILawyersService>();
        public LawyerControllerTests()
        {
            _sut = new LawyerController(_mockLawyersService);
        }

        private static IEnumerable<LawyerListItemModel> GetSampleData_ListItems()
        {
            var result = new List<LawyerListItemModel>
            {
                new LawyerListItemModel { LawyerId = 1, LawyerNameAndEmail = "ABC (abc@xyz.com)" },
                new LawyerListItemModel { LawyerId = 2, LawyerNameAndEmail = "DEF (def@xyz.com)" }
            };
            return result;
        }

        private static LawyerDetailsModel GetSampleData_Details(int lawyerId)
        {
            var result = new LawyerDetailsModel
            {
                LawyerId = lawyerId,
                LawyerFullname = "ABC",
                LawyerEmail = "abc@xyz.com",
                LawyerAddress = "Earth",
                LawyerPhone = "1234"
            };
            return result;
        }

        [Fact]
        public async Task GetAllLawyers_Valid()
        {
            // Arrange
            IEnumerable<LawyerListItemModel> expected = GetSampleData_ListItems();
            _mockLawyersService.RetrieveAllAsync().Returns(expected);

            // Act
            ActionResult<IEnumerable<LawyerListItemModel>> response = await _sut.GetAllLawyers();

            // Assert
            await _mockLawyersService.Received(1).RetrieveAllAsync();
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<LawyerListItemModel> actual = (IEnumerable<LawyerListItemModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).LawyerId, actual.ElementAt(i).LawyerId);
                Assert.Equal(expected.ElementAt(i).LawyerNameAndEmail, actual.ElementAt(i).LawyerNameAndEmail);
            }
        }

        [Fact]
        public async Task GetLawyerDetails_UnprocessableEntity()
        {
            // Arrange
            int lawyerId = -1;
            string expectedError = $"Invalid LawyerId: {lawyerId}";

            // Act
            ActionResult<LawyerDetailsModel> response = await _sut.GetLawyerDetails(lawyerId);

            // Assert
            await _mockLawyersService.DidNotReceiveWithAnyArgs().RetrieveAsync(lawyerId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetLawyerDetails_NotFound()
        {
            // Arrange
            int lawyerId = 1;
            LawyerDetailsModel expected = null;
            _mockLawyersService.RetrieveAsync(lawyerId).Returns(expected);

            // Act
            ActionResult<LawyerDetailsModel> response = await _sut.GetLawyerDetails(lawyerId);

            // Assert
            await _mockLawyersService.Received(1).RetrieveAsync(lawyerId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetLawyerDetails_Valid()
        {
            // Arrange
            int lawyerId = 1;
            LawyerDetailsModel expected = GetSampleData_Details(lawyerId);
            _mockLawyersService.RetrieveAsync(lawyerId).Returns(expected);

            // Act
            ActionResult<LawyerDetailsModel> response = await _sut.GetLawyerDetails(lawyerId);

            // Assert
            await _mockLawyersService.Received(1).RetrieveAsync(lawyerId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            LawyerDetailsModel actual = (LawyerDetailsModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.LawyerId, actual.LawyerId);
            Assert.Equal(expected.LawyerFullname, actual.LawyerFullname);
            Assert.Equal(expected.LawyerEmail, actual.LawyerEmail);
            Assert.Equal(expected.LawyerAddress, actual.LawyerAddress);
            Assert.Equal(expected.LawyerPhone, actual.LawyerPhone);
        }

        [Fact]
        public async Task CreateNewLawyer_ValidationProblem()
        {
            // Arrange
            NewLawyerModel lawyerModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.CreateNewLawyer(lawyerModel);

            // Assert
            await _mockLawyersService.DidNotReceiveWithAnyArgs().CreateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task CreateNewLawyer_Valid()
        {
            // Arrange
            NewLawyerModel lawyerModel = new()
            {
                LawyerEmail = "abc@xyz.com",
                LawyerFullname = "ABC",
                LawyerAddress = "Earth",
                LawyerPhone = "1234"
            };
            int lawyerId = 1;
            _mockLawyersService.CreateAsync(default).ReturnsForAnyArgs(lawyerId);

            // Act
            IActionResult response = await _sut.CreateNewLawyer(lawyerModel);

            // Assert
            await _mockLawyersService.Received().CreateAsync(Arg.Is<NewLawyerModel>(p
                => p.LawyerEmail == lawyerModel.LawyerEmail && p.LawyerFullname == lawyerModel.LawyerFullname
                && p.LawyerPhone == lawyerModel.LawyerPhone && p.LawyerAddress == lawyerModel.LawyerAddress));
            var createdAtActionResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(lawyerId, (int)createdAtActionResult.Value);
            Assert.Equal("api/config/Lawyer", createdAtActionResult.Location);
        }

        [Fact]
        public async Task UpdateLawyerDetails_ValidationProblem()
        {
            // Arrange
            LawyerDetailsModel lawyerModel = new();
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateLawyerDetails(lawyerModel);

            // Assert
            await _mockLawyersService.DidNotReceiveWithAnyArgs().UpdateAsync(default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateLawyerDetails_Valid()
        {
            // Arrange
            int updateAsyncCalled = 0;
            LawyerDetailsModel lawyerModel = new()
            {
                LawyerId = 1,
                LawyerEmail = "abc@xyz.com",
                LawyerFullname = "ABC",
                LawyerAddress = "Earth",
                LawyerPhone = "1234"
            };
            _mockLawyersService.WhenForAnyArgs(x => x.UpdateAsync(default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateLawyerDetails(lawyerModel);

            // Assert
            await _mockLawyersService.Received().UpdateAsync(Arg.Is<LawyerDetailsModel>(p => p.LawyerId == lawyerModel.LawyerId
                && p.LawyerEmail == lawyerModel.LawyerEmail && p.LawyerFullname == lawyerModel.LawyerFullname
                && p.LawyerPhone == lawyerModel.LawyerPhone && p.LawyerAddress == lawyerModel.LawyerAddress));
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int lawyerId = 0;
            string expectedError = $"Invalid LawyerId: {lawyerId}";

            // Act
            IActionResult response = await _sut.Delete(lawyerId);

            // Assert
            await _mockLawyersService.DidNotReceiveWithAnyArgs().DeleteAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int lawyerId = 1;
            int deleteAsyncCalled = 0;
            _mockLawyersService.WhenForAnyArgs(x => x.DeleteAsync(lawyerId))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(lawyerId);

            // Assert
            await _mockLawyersService.Received(1).DeleteAsync(lawyerId);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
