using CCMS.Server.Controllers.CaseControllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Enums;
using CCMS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.CaseControllers
{
    public class ProceedingsControllerTests
    {
        private readonly ProceedingsController _sut;
        private readonly ICaseProceedingsService _mockCaseProceedingsService = Substitute.For<ICaseProceedingsService>();
        private readonly IProceedingDecisionsService _mockProceedingDecisionsService = Substitute.For<IProceedingDecisionsService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public ProceedingsControllerTests()
        {
            _sut = new ProceedingsController(_mockCaseProceedingsService, _mockProceedingDecisionsService, _mockSessionService);
        }
        private static IEnumerable<CaseProceedingModel> GetSampleData_CaseProceedings()
        {
            var result = new List<CaseProceedingModel>
            {
                new CaseProceedingModel { CaseProceedingId = 1, ProceedingDate = DateTime.Today.AddDays(-1), NextHearingOn = DateTime.Today, ProceedingDecision = 1, JudgementFile = 0 },
                new CaseProceedingModel { CaseProceedingId = 2, ProceedingDate = DateTime.Today, NextHearingOn = DateTime.Today.AddDays(1), ProceedingDecision = 0, JudgementFile = 0 }
            };
            return result;
        }
        private static IEnumerable<AssignedProceedingModel> GetSampleData_AssignedProceedings()
        {
            var result = new List<AssignedProceedingModel>
            {
                new AssignedProceedingModel { CaseProceedingId = 1, CaseNumber = "CN1", AppealNumber = 1, ProceedingDate = DateTime.Today.AddDays(-1), NextHearingOn = DateTime.Today, CaseStatus = "PENDING", AssignedTo = "Abc (abc@xyz.com)" },
                new AssignedProceedingModel { CaseProceedingId = 2, CaseNumber = "CN2", AppealNumber = 0, ProceedingDate = DateTime.Today.AddDays(-1), NextHearingOn = DateTime.Today, CaseStatus = "PENDING", AssignedTo = "Abc (abc@xyz.com)" },
            };
            return result;
        }

        private static bool IsEqual(CaseProceedingModel expected, CaseProceedingModel actual)
        {
            bool isEqual = expected.CaseProceedingId == actual.CaseProceedingId
                && expected.ProceedingDate == actual.ProceedingDate
                && expected.NextHearingOn == actual.NextHearingOn
                && expected.ProceedingDecision == actual.ProceedingDecision
                && expected.JudgementFile == actual.JudgementFile;
            return isEqual;
        }

        [Fact]
        public async Task GetProceedingDetails_UnprocessableEntity()
        {
            // Arrange
            int caseProceedingId = 0;
            string expectedError = $"Invalid CaseProceedingId: {caseProceedingId}";

            // Act
            ActionResult<CaseProceedingModel> response = await _sut.GetProceedingDetails(caseProceedingId);

            // Assert
            await _mockCaseProceedingsService.DidNotReceive().RetrieveAsync(caseProceedingId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetProceedingDetails_NotFound()
        {
            // Arrange
            int caseProceedingId = 1;
            CaseProceedingModel expected = null;
            _mockCaseProceedingsService.RetrieveAsync(caseProceedingId).Returns(expected);

            // Act
            ActionResult<CaseProceedingModel> response = await _sut.GetProceedingDetails(caseProceedingId);

            // Assert
            await _mockCaseProceedingsService.Received(1).RetrieveAsync(caseProceedingId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetProceedingDetails_Valid()
        {
            // Arrange
            int caseProceedingId = 1;
            CaseProceedingModel expected = GetSampleData_CaseProceedings().FirstOrDefault(x => x.CaseProceedingId == caseProceedingId);
            _mockCaseProceedingsService.RetrieveAsync(caseProceedingId).Returns(expected);

            // Act
            ActionResult<CaseProceedingModel> response = await _sut.GetProceedingDetails(caseProceedingId);

            // Assert
            await _mockCaseProceedingsService.Received(1).RetrieveAsync(caseProceedingId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            CaseProceedingModel actual = (CaseProceedingModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseProceedingId, actual.CaseProceedingId);
            Assert.Equal(expected.ProceedingDate, actual.ProceedingDate);
            Assert.Equal(expected.NextHearingOn, actual.NextHearingOn);
            Assert.Equal(expected.ProceedingDecision, actual.ProceedingDecision);
            Assert.Equal(expected.JudgementFile, actual.JudgementFile);
        }

        [Fact]
        public async Task GetCaseProceedings_UnprocessableEntity()
        {
            // Arrange
            int caseId = 0;
            string expectedError = $"Invalid CaseId: {caseId}";

            // Act
            ActionResult<IEnumerable<CaseProceedingModel>> response = await _sut.GetCaseProceedings(caseId);

            // Assert
            await _mockCaseProceedingsService.DidNotReceive().RetrieveAllCaseProceedingsAsync(caseId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCaseProceedings_NotFound()
        {
            // Arrange
            int caseId = 1;
            IEnumerable<CaseProceedingModel> expected = null;
            _mockCaseProceedingsService.RetrieveAllCaseProceedingsAsync(caseId).Returns(expected);

            // Act
            ActionResult<IEnumerable<CaseProceedingModel>> response = await _sut.GetCaseProceedings(caseId);

            // Assert
            await _mockCaseProceedingsService.Received(1).RetrieveAllCaseProceedingsAsync(caseId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCaseProceedings_Valid()
        {
            // Arrange
            int caseId = 1;
            IEnumerable<CaseProceedingModel> expected = GetSampleData_CaseProceedings();
            _mockCaseProceedingsService.RetrieveAllCaseProceedingsAsync(caseId).Returns(expected);

            // Act
            ActionResult<IEnumerable<CaseProceedingModel>> response = await _sut.GetCaseProceedings(caseId);

            // Assert
            await _mockCaseProceedingsService.Received(1).RetrieveAllCaseProceedingsAsync(caseId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<CaseProceedingModel> actual = (IEnumerable<CaseProceedingModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseProceedingId, actual.ElementAt(i).CaseProceedingId);
                Assert.Equal(expected.ElementAt(i).ProceedingDate, actual.ElementAt(i).ProceedingDate);
                Assert.Equal(expected.ElementAt(i).NextHearingOn, actual.ElementAt(i).NextHearingOn);
                Assert.Equal(expected.ElementAt(i).ProceedingDecision, actual.ElementAt(i).ProceedingDecision);
                Assert.Equal(expected.ElementAt(i).JudgementFile, actual.ElementAt(i).JudgementFile);
            }
        }

        [Fact]
        public async Task GetAssignedProceedings_UnprocessableEntity()
        {
            // Arrange
            int userId = -1;
            string expectedError = $"Invalid UserId: {userId}";

            // Act
            ActionResult<IEnumerable<AssignedProceedingModel>> response = await _sut.GetAssignedProceedings(userId);

            // Assert
            await _mockCaseProceedingsService.DidNotReceive().RetrieveAllCaseProceedingsAsync(userId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetAssignedProceedings_Valid()
        {
            // Arrange
            int userId = 1;
            IEnumerable<AssignedProceedingModel> expected = GetSampleData_AssignedProceedings();
            _mockCaseProceedingsService.RetrieveAssignedProceedingsAsync(userId).Returns(expected);

            // Act
            ActionResult<IEnumerable<AssignedProceedingModel>> response = await _sut.GetAssignedProceedings(userId);

            // Assert
            await _mockCaseProceedingsService.Received(1).RetrieveAssignedProceedingsAsync(userId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<AssignedProceedingModel> actual = (IEnumerable<AssignedProceedingModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseProceedingId, actual.ElementAt(i).CaseProceedingId);
                Assert.Equal(expected.ElementAt(i).CaseNumber, actual.ElementAt(i).CaseNumber);
                Assert.Equal(expected.ElementAt(i).AppealNumber, actual.ElementAt(i).AppealNumber);
                Assert.Equal(expected.ElementAt(i).ProceedingDate, actual.ElementAt(i).ProceedingDate);
                Assert.Equal(expected.ElementAt(i).NextHearingOn, actual.ElementAt(i).NextHearingOn);
                Assert.Equal(expected.ElementAt(i).CaseStatus, actual.ElementAt(i).CaseStatus);
                Assert.Equal(expected.ElementAt(i).AssignedTo, actual.ElementAt(i).AssignedTo);
            }
        }

        [Fact]
        public async Task UpdateCaseProceedingDetails_ValidationProblem()
        {
            // Arrange
            CaseProceedingModel caseProceedingModel = GetSampleData_CaseProceedings().FirstOrDefault(x => x.CaseProceedingId == 1);
            _sut.ModelState.AddModelError("Field", "Sample Error Proceedings");

            // Act
            await _sut.UpdateCaseProceedingDetails(caseProceedingModel);

            // Assert
            await _mockProceedingDecisionsService.DidNotReceiveWithAnyArgs().RetrieveAsync(default);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCaseProceedingsService.DidNotReceiveWithAnyArgs().UpdateAsync(default, default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Theory]
        [InlineData(true, false, true, 1)]
        [InlineData(false, true, true, 1)]
        [InlineData(true, true, true, 0)]
        [InlineData(true, true, false, 1)]
        [InlineData(true, false, false, 1)]
        [InlineData(false, true, false, 1)]
        [InlineData(false, true, true, 0)]
        public async Task UpdateCaseProceedings_UnprocessableEntity(bool hasNextHearingDate, bool nextHearingValue,
            bool hasOrderAttachment, int judgementFile)
        {
            // Arrange
            CaseProceedingModel caseProceedingModel = GetSampleData_CaseProceedings().FirstOrDefault(x => x.CaseProceedingId == 1);
            caseProceedingModel.NextHearingOn = nextHearingValue ? DateTime.Now.AddDays(1) : null;
            caseProceedingModel.JudgementFile = judgementFile;
            ProceedingDecisionModel proceedingDecision = new()
            {
                ProceedingDecisionId = caseProceedingModel.ProceedingDecision,
                HasNextHearingDate = hasNextHearingDate,
                HasOrderAttachment = hasOrderAttachment
            };
            _mockProceedingDecisionsService.RetrieveAsync(caseProceedingModel.ProceedingDecision).Returns(proceedingDecision);
            string expectedError = "Proceeding Decision conditions not met";

            // Act
            IActionResult response = await _sut.UpdateCaseProceedingDetails(caseProceedingModel);

            // Assert
            await _mockProceedingDecisionsService.Received(1).RetrieveAsync(caseProceedingModel.ProceedingDecision);
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCaseProceedingsService.DidNotReceiveWithAnyArgs().UpdateAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }
        
        [Fact]
        public async Task UpdateCaseProceedings_Valid()
        {
            // Arrange
            CaseProceedingModel caseProceedingModel = GetSampleData_CaseProceedings().FirstOrDefault(x => x.CaseProceedingId == 1);
            ProceedingDecisionModel proceedingDecision = new()
            {
                ProceedingDecisionId = caseProceedingModel.ProceedingDecision,
                HasNextHearingDate = true,
                HasOrderAttachment = false
            };
            int currUser = 1;
            int updateAsyncCalled = 0;
            _mockProceedingDecisionsService.RetrieveAsync(caseProceedingModel.ProceedingDecision).Returns(proceedingDecision);
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockCaseProceedingsService.WhenForAnyArgs(x => x.UpdateAsync(default, default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateCaseProceedingDetails(caseProceedingModel);

            // Assert
            await _mockProceedingDecisionsService.Received(1).RetrieveAsync(caseProceedingModel.ProceedingDecision);
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockCaseProceedingsService.ReceivedWithAnyArgs(1).UpdateAsync(Arg.Is<CaseProceedingModel>(
                p => IsEqual(caseProceedingModel, p)), currUser);
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
        
        [Fact]
        public async Task AssignCaseProceeding_Valid()
        {
            // Arrange
            int caseProceedingId = 1;
            int assignTo = 2;
            int currUser = 1;
            int assignProceedingAsyncCalled = 0;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockCaseProceedingsService.WhenForAnyArgs(x => x.AssignProceedingAsync(caseProceedingId, assignTo, currUser))
                .Do(x => assignProceedingAsyncCalled++);

            // Act
            IActionResult response = await _sut.AssignCaseProceeding(caseProceedingId, assignTo);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockCaseProceedingsService.Received(1).AssignProceedingAsync(caseProceedingId, assignTo, currUser);
            Assert.Equal(1, assignProceedingAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int caseProceedingId = 0;
            string expectedError = $"Invalid CaseProceedingId: {caseProceedingId}";

            // Act
            IActionResult response = await _sut.Delete(caseProceedingId);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockCaseProceedingsService.DidNotReceiveWithAnyArgs().DeleteAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int caseProceedingId = 1;
            int currUser = 1;
            int deleteAsyncCalled = 0;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockCaseProceedingsService.WhenForAnyArgs(x => x.DeleteAsync(caseProceedingId, currUser))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(caseProceedingId);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockCaseProceedingsService.Received(1).DeleteAsync(caseProceedingId, currUser);
            Assert.Equal(1, deleteAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
