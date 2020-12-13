using CCMS.Server.Controllers.CaseControllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
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
    public class DatesControllerTests
    {
        private readonly DatesController _sut;
        private readonly ICaseDatesService _mockDatesService = Substitute.For<ICaseDatesService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public DatesControllerTests()
        {
            _sut = new DatesController(_mockDatesService, _mockSessionService);
        }

        private static CaseDatesModel GetSampleData(int caseId)
        {
            var result = new CaseDatesModel
            {
                CaseId = caseId,
                CaseFiledOn = DateTime.Today,
                NoticeReceivedOn = DateTime.Today,
                FirstHearingOn = DateTime.Today.AddDays(1)
            };
            return result;
        }

        private static bool IsEqual(CaseDatesModel expected, CaseDatesModel actual)
        {
            bool isEqual = expected.CaseId == actual.CaseId
                && expected.CaseFiledOn == actual.CaseFiledOn
                && expected.NoticeReceivedOn == actual.NoticeReceivedOn
                && expected.FirstHearingOn == actual.FirstHearingOn;
            return isEqual;
        }

        [Fact]
        public async Task GetCaseDateDetails_UnprocessableEntity()
        {
            // Arrange
            int caseId = 0;
            string expectedError = $"Invalid CaseId: {caseId}";

            // Act
            ActionResult<CaseDatesModel> response = await _sut.GetCaseDateDetails(caseId);

            // Assert
            await _mockDatesService.DidNotReceive().RetrieveAsync(caseId);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetCaseDateDetails_NotFound()
        {
            // Arrange
            int caseId = 1;
            CaseDatesModel expected = null;
            _mockDatesService.RetrieveAsync(caseId).Returns(expected);

            // Act
            ActionResult<CaseDatesModel> response = await _sut.GetCaseDateDetails(caseId);

            // Assert
            await _mockDatesService.Received(1).RetrieveAsync(caseId);
            Assert.IsType<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task GetCaseDateDetails_Valid()
        {
            // Arrange
            int caseId = 1;
            CaseDatesModel expected = GetSampleData(caseId);
            _mockDatesService.RetrieveAsync(caseId).Returns(expected);

            // Act
            ActionResult<CaseDatesModel> response = await _sut.GetCaseDateDetails(caseId);

            // Assert
            await _mockDatesService.Received(1).RetrieveAsync(caseId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            CaseDatesModel actual = (CaseDatesModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseId, actual.CaseId);
            Assert.Equal(expected.CaseFiledOn, actual.CaseFiledOn);
            Assert.Equal(expected.NoticeReceivedOn, actual.NoticeReceivedOn);
            Assert.Equal(expected.FirstHearingOn, actual.FirstHearingOn);
        }

        [Fact]
        public async Task UpdateCaseDateDetails_ValidationProblem()
        {
            // Arrange
            int caseId = 1;
            CaseDatesModel caseDateModel = GetSampleData(caseId);
            _sut.ModelState.AddModelError("Field", "Sample Error Details");

            // Act
            await _sut.UpdateCaseDateDetails(caseDateModel);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockDatesService.DidNotReceiveWithAnyArgs().UpdateAsync(default, default);
            Assert.False(_sut.ModelState.IsValid);
        }

        [Fact]
        public async Task UpdateCaseDateDetails_Valid()
        {
            // Arrange
            int caseId = 1;
            int currUser = 1;
            int updateAsyncCalled = 0;
            CaseDatesModel caseDateModel = GetSampleData(caseId);
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockDatesService.WhenForAnyArgs(x => x.UpdateAsync(default, default))
                .Do(x => updateAsyncCalled++);

            // Act
            IActionResult response = await _sut.UpdateCaseDateDetails(caseDateModel);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockDatesService.ReceivedWithAnyArgs(1).UpdateAsync(Arg.Is<CaseDatesModel>(
                p => IsEqual(caseDateModel, p)), currUser);
            Assert.Equal(1, updateAsyncCalled);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
