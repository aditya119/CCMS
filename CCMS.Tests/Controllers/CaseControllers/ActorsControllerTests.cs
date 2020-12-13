using CCMS.Server.Controllers.CaseControllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.CaseControllers
{
    public class ActorsControllerTests
    {
        private readonly ActorsController _sut;
        private readonly ICaseActorsService _mockActorsService = Substitute.For<ICaseActorsService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public ActorsControllerTests()
        {
            _sut = new ActorsController(_mockActorsService, _mockSessionService);
        }

        private static IEnumerable<CaseActorModel> GetSampleData(int caseId)
        {
            var result = new List<CaseActorModel>
            {
                new CaseActorModel { CaseId = caseId, ActorTypeId = 1, ActorName = "Abc", ActorAddress = "xyz", ActorEmail = "abc@xyz.com", ActorPhone = "12", DetailFile = 0 },
                new CaseActorModel { CaseId = caseId, ActorTypeId = 2, ActorName = "Defc", ActorAddress = "xyz", ActorEmail = "def@xyz.com", ActorPhone = "234", DetailFile = 0 }
            };
            return result;
        }

        private static bool IsEqual(IEnumerable<CaseActorModel> expected, IEnumerable<CaseActorModel> actual)
        {
            bool isEqual = false;
            if (expected.Count() != actual.Count())
            {
                return isEqual;
            }
            for (int i = 0; i < expected.Count(); i++)
            {
                isEqual = expected.ElementAt(i).CaseId == actual.ElementAt(i).CaseId
                    && expected.ElementAt(i).ActorTypeId == actual.ElementAt(i).ActorTypeId
                    && expected.ElementAt(i).ActorName == actual.ElementAt(i).ActorName
                    && expected.ElementAt(i).ActorAddress == actual.ElementAt(i).ActorAddress
                    && expected.ElementAt(i).ActorEmail == actual.ElementAt(i).ActorEmail
                    && expected.ElementAt(i).ActorPhone == actual.ElementAt(i).ActorPhone;
                if (isEqual == false)
                {
                    return isEqual;
                }
            }
            return isEqual;
        }

        [Fact]
        public async Task GetCaseActorDetails_UnprocessableEntity()
        {
            // Arrange
            int caseId = 0;
            string expectedError = $"Invalid CaseId: {caseId}";

            // Act
            ActionResult<IEnumerable<CaseActorModel>> response = await _sut.GetCaseActorDetails(caseId);

            // Assert
            await _mockActorsService.DidNotReceive().RetrieveAsync(caseId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCaseActorDetails_NotFound()
        {
            // Arrange
            int caseId = 1;
            IEnumerable<CaseActorModel> expected = null;
            _mockActorsService.RetrieveAsync(caseId).Returns(expected);

            // Act
            ActionResult<IEnumerable<CaseActorModel>> response = await _sut.GetCaseActorDetails(caseId);

            // Assert
            await _mockActorsService.Received(1).RetrieveAsync(caseId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCaseActorDetails_Valid()
        {
            // Arrange
            int caseId = 1;
            IEnumerable<CaseActorModel> expected = GetSampleData(caseId);
            _mockActorsService.RetrieveAsync(caseId).Returns(expected);

            // Act
            ActionResult<IEnumerable<CaseActorModel>> response = await _sut.GetCaseActorDetails(caseId);

            // Assert
            await _mockActorsService.Received(1).RetrieveAsync(caseId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<CaseActorModel> actual = (IEnumerable<CaseActorModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseId, actual.ElementAt(i).CaseId);
                Assert.Equal(expected.ElementAt(i).ActorTypeId, actual.ElementAt(i).ActorTypeId);
                Assert.Equal(expected.ElementAt(i).ActorName, actual.ElementAt(i).ActorName);
                Assert.Equal(expected.ElementAt(i).ActorAddress, actual.ElementAt(i).ActorAddress);
                Assert.Equal(expected.ElementAt(i).ActorEmail, actual.ElementAt(i).ActorEmail);
                Assert.Equal(expected.ElementAt(i).ActorPhone, actual.ElementAt(i).ActorPhone);
            }
        }

        [Fact]
        public async Task UpdateCaseActorDetails_ValidationProblem()
        {
            // Arrange
            int caseId = 1;
            IEnumerable<CaseActorModel> caseActorModels = GetSampleData(caseId);
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateCaseActorDetails(caseActorModels);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockActorsService.DidNotReceiveWithAnyArgs().UpdateAsync(default, default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateCaseActorDetails_Valid()
        {
            // Arrange
            int caseId = 1;
            int currUser = 1;
            int updateAsyncCalled = 0;
            IEnumerable<CaseActorModel> caseActorModels = GetSampleData(caseId);
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockActorsService.WhenForAnyArgs(x => x.UpdateAsync(default, default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateCaseActorDetails(caseActorModels);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockActorsService.ReceivedWithAnyArgs(1).UpdateAsync(Arg.Is<IEnumerable<CaseActorModel>>(
                p => IsEqual(caseActorModels, p)), currUser);
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
