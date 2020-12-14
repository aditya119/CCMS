using CCMS.Server.Controllers.CaseControllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Enums;
using CCMS.Shared.Models.CourtCaseModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.CaseControllers
{
    public class DetailsControllerTests
    {
        private readonly DetailsController _sut;
        private readonly ICourtCasesService _mockCourtCasesService = Substitute.For<ICourtCasesService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public DetailsControllerTests()
        {
            _sut = new DetailsController(_mockCourtCasesService, _mockSessionService);
        }

        private static CaseDetailsModel GetSampleData_CaseDetails(int caseId)
        {
            var result = new CaseDetailsModel
            {
                CaseId = caseId,
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1,
                CaseStatus = 1
            };
            return result;
        }

        private static CaseStatusModel GetSampleData_CaseStatus()
        {
            var result = new CaseStatusModel
            {
                StatusId = 1,
                StatusName = "FINAL JUDGEMENT"
            };
            return result;
        }

        private static bool IsEqual(UpdateCaseModel expected, UpdateCaseModel actual)
        {
            bool isEqual = expected.CaseId == actual.CaseId
                && expected.CaseNumber == actual.CaseNumber
                && expected.AppealNumber == actual.AppealNumber
                && expected.CaseTypeId == actual.CaseTypeId
                && expected.CourtId == actual.CourtId
                && expected.LawyerId == actual.LawyerId
                && expected.LawyerId == actual.LawyerId
                && expected.LocationId == actual.LocationId;
            return isEqual;
        }

        private static bool IsEqual(NewCaseModel expected, NewCaseModel actual)
        {
            bool isEqual = expected.CaseNumber == actual.CaseNumber
                && expected.AppealNumber == actual.AppealNumber
                && expected.CaseTypeId == actual.CaseTypeId
                && expected.CourtId == actual.CourtId
                && expected.LawyerId == actual.LawyerId
                && expected.LawyerId == actual.LawyerId
                && expected.LocationId == actual.LocationId;
            return isEqual;
        }

        [Fact]
        public async Task GetCaseDetails_UnprocessableEntity()
        {
            // Arrange
            int caseId = 0;
            string expectedError = $"Invalid CaseId: {caseId}";

            // Act
            ActionResult<CaseDetailsModel> response = await _sut.GetCaseDetails(caseId);

            // Assert
            await _mockCourtCasesService.DidNotReceive().RetrieveAsync(caseId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCaseDetails_NotFound()
        {
            // Arrange
            int caseId = 1;
            CaseDetailsModel expected = null;
            _mockCourtCasesService.RetrieveAsync(caseId).Returns(expected);

            // Act
            ActionResult<CaseDetailsModel> response = await _sut.GetCaseDetails(caseId);

            // Assert
            await _mockCourtCasesService.Received(1).RetrieveAsync(caseId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCaseDetails_Valid()
        {
            // Arrange
            int caseId = 1;
            CaseDetailsModel expected = GetSampleData_CaseDetails(caseId);
            _mockCourtCasesService.RetrieveAsync(caseId).Returns(expected);

            // Act
            ActionResult<CaseDetailsModel> response = await _sut.GetCaseDetails(caseId);

            // Assert
            await _mockCourtCasesService.Received(1).RetrieveAsync(caseId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            CaseDetailsModel actual = (CaseDetailsModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseId, actual.CaseId);
            Assert.Equal(expected.CaseId, actual.CaseId);
            Assert.Equal(expected.CaseNumber, actual.CaseNumber);
            Assert.Equal(expected.AppealNumber, actual.AppealNumber);
            Assert.Equal(expected.CaseStatus, actual.CaseStatus);
            Assert.Equal(expected.CaseTypeId, actual.CaseTypeId);
            Assert.Equal(expected.CourtId, actual.CourtId);
            Assert.Equal(expected.LawyerId, actual.LawyerId);
            Assert.Equal(expected.LocationId, actual.LocationId);
        }

        [Fact]
        public async Task GetCaseStatus_UnprocessableEntity()
        {
            // Arrange
            int caseId = 0;
            string expectedError = $"Invalid CaseId: {caseId}";

            // Act
            ActionResult<CaseStatusModel> response = await _sut.GetCaseStatus(caseId);

            // Assert
            await _mockCourtCasesService.DidNotReceive().GetCaseStatusAsync(caseId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCaseStatus_NotFound()
        {
            // Arrange
            int caseId = 1;
            CaseStatusModel expected = null;
            _mockCourtCasesService.GetCaseStatusAsync(caseId).Returns(expected);

            // Act
            ActionResult<CaseStatusModel> response = await _sut.GetCaseStatus(caseId);

            // Assert
            await _mockCourtCasesService.Received(1).GetCaseStatusAsync(caseId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCaseStatus_Valid()
        {
            // Arrange
            int caseId = 1;
            CaseStatusModel expected = GetSampleData_CaseStatus();
            _mockCourtCasesService.GetCaseStatusAsync(caseId).Returns(expected);

            // Act
            ActionResult<CaseStatusModel> response = await _sut.GetCaseStatus(caseId);

            // Assert
            await _mockCourtCasesService.Received(1).GetCaseStatusAsync(caseId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            CaseStatusModel actual = (CaseStatusModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.StatusId, actual.StatusId);
            Assert.Equal(expected.StatusName, actual.StatusName);
        }

        [Fact]
        public async Task CreateNewCase_ValidationProblem()
        {
            // Arrange
            var caseModel = new NewCaseModel
            {
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.CreateNewCase(caseModel);

            // Assert
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().ExistsCaseNumberAsync(default, default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().GetCaseStatusAsync(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().CreateAsync(default, default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task CreateNewCase_UnprocessableEntity_CaseAlreadyExists()
        {
            // Arrange
            var caseModel = new NewCaseModel
            {
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            int caseId = 1;
            _mockCourtCasesService.ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber).Returns((caseId, null));
            string expectedError = "Case already exists";

            // Act
            IActionResult response = await _sut.CreateNewCase(caseModel);

            // Assert
            await _mockCourtCasesService.Received(1).ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().GetCaseStatusAsync(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().CreateAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task CreateNewCase_UnprocessableEntity_PrevAppealDoesNotExist(bool existsPrevAppeal)
        {
            // Arrange
            var caseModel = new NewCaseModel
            {
                CaseNumber = "Cn1",
                AppealNumber = 1,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            (int, DateTime?) returnTuple = existsPrevAppeal switch
            {
                true => (1, DateTime.Now),
                _ => (-1, null)
            };
            _mockCourtCasesService.ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber).Returns((-1, null));
            _mockCourtCasesService.ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber - 1).Returns(returnTuple);
            string expectedError = "Previous appeal does not exist";

            // Act
            IActionResult response = await _sut.CreateNewCase(caseModel);

            // Assert
            await _mockCourtCasesService.Received(2).ExistsCaseNumberAsync(caseModel.CaseNumber,
                Arg.Is<int>(p => p == caseModel.AppealNumber || p == caseModel.AppealNumber - 1));
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().GetCaseStatusAsync(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().CreateAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateNewCase_UnprocessableEntity_PrevAppealNotFinalJudgment()
        {
            // Arrange
            var caseModel = new NewCaseModel
            {
                CaseNumber = "Cn1",
                AppealNumber = 1,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            var caseStatus = new CaseStatusModel
            {
                StatusId = 1,
                StatusName = ProceedingDecisions.Adjournment
            };
            (int caseId, DateTime?) prevAppeal = (1, null);
            _mockCourtCasesService.ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber).Returns((-1, null));
            _mockCourtCasesService.ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber - 1).Returns(prevAppeal);
            _mockCourtCasesService.GetCaseStatusAsync(prevAppeal.caseId).Returns(caseStatus);
            string expectedError = $"Previous appeal status, {caseStatus.StatusName}, must be {ProceedingDecisions.FinalJudgement}";

            // Act
            IActionResult response = await _sut.CreateNewCase(caseModel);

            // Assert
            await _mockCourtCasesService.Received(2).ExistsCaseNumberAsync(caseModel.CaseNumber,
                Arg.Is<int>(p => p == caseModel.AppealNumber || p == caseModel.AppealNumber - 1));
            await _mockCourtCasesService.Received(1).GetCaseStatusAsync(prevAppeal.caseId);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().CreateAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateNewCase_Valid()
        {
            // Arrange
            var caseModel = new NewCaseModel
            {
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            int caseId = 1;
            int currUser = 1;
            _mockCourtCasesService.ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber).Returns((-1, null));
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockCourtCasesService.CreateAsync(default, currUser).ReturnsForAnyArgs(caseId);

            // Act
            IActionResult response = await _sut.CreateNewCase(caseModel);

            // Assert
            await _mockCourtCasesService.Received(1).ExistsCaseNumberAsync(caseModel.CaseNumber, caseModel.AppealNumber);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().GetCaseStatusAsync(default);
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockCourtCasesService.ReceivedWithAnyArgs().CreateAsync(Arg.Is<NewCaseModel>(p => IsEqual(caseModel, p)), currUser);
            var createdAtActionResult = Assert.IsType<CreatedResult>(response);
            Assert.Equal(caseId, (int)createdAtActionResult.Value);
            Assert.Equal("api/Case/Details", createdAtActionResult.Location);
        }

        [Fact]
        public async Task UpdateCaseDetails_ValidationProblem()
        {
            // Arrange
            var caseModel = new UpdateCaseModel
            {
                CaseId = 1,
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateCaseDetails(caseModel);

            // Assert
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().ExistsCaseIdAsync(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().UpdateAsync(default, default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task UpdateCaseDetails_UnprocessableEntity(bool caseExists)
        {
            // Arrange
            var caseModel = new UpdateCaseModel
            {
                CaseId = 1,
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            string expectedError = "Case deleted or does not exist";
            (string, int, DateTime?) returnTuple = caseExists switch
            {
                true => ("Cn1", 1, DateTime.Now),
                _ => (null, -1, null)
            };
            _mockCourtCasesService.ExistsCaseIdAsync(caseModel.CaseId).Returns(returnTuple);

            // Act
            IActionResult response = await _sut.UpdateCaseDetails(caseModel);

            // Assert
            await _mockCourtCasesService.Received(1).ExistsCaseIdAsync(caseModel.CaseId);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task UpdateCaseDetails_Valid()
        {
            // Arrange
            var caseModel = new UpdateCaseModel
            {
                CaseId = 1,
                CaseNumber = "Cn1",
                AppealNumber = 0,
                CaseTypeId = 1,
                CourtId = 1,
                LawyerId = 1,
                LocationId = 1
            };
            int currUser = 1;
            int updateAsyncCalled = 0;
            _mockCourtCasesService.ExistsCaseIdAsync(caseModel.CaseId).Returns((caseModel.CaseNumber, caseModel.AppealNumber, null));
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockCourtCasesService.WhenForAnyArgs(x => x.UpdateAsync(default, default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateCaseDetails(caseModel);

            // Assert
            await _mockCourtCasesService.Received(1).ExistsCaseIdAsync(caseModel.CaseId);
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockCourtCasesService.ReceivedWithAnyArgs(1).UpdateAsync(Arg.Is<UpdateCaseModel>(
                p => IsEqual(caseModel, p)), currUser);
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int caseId = 0;
            string expectedError = $"Invalid CaseId: {caseId}";

            // Act
            IActionResult response = await _sut.Delete(caseId);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCourtCasesService.DidNotReceiveWithAnyArgs().DeleteAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int caseId = 1;
            int currUser = 1;
            int deleteAsyncCalled = 0;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockCourtCasesService.WhenForAnyArgs(x => x.DeleteAsync(caseId, currUser))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(caseId);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockCourtCasesService.Received(1).DeleteAsync(caseId, currUser);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
