using CCMS.Server.Controllers.CaseControllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.InsightsModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers.CaseControllers
{
    public class InsightsControllerTests
    {
        private readonly InsightsController _sut;
        private readonly IInsightsService _mockInsightsService = Substitute.For<IInsightsService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public InsightsControllerTests()
        {
            _sut = new InsightsController(_mockInsightsService, _mockSessionService);
        }

        private static PendingDisposedCountModel GetSampleData_PendingDisposedCount()
        {
            return new PendingDisposedCountModel
            {
                DisposedOff = 1,
                Pending = 3
            };
        }

        private static IEnumerable<ParameterisedReportModel> GetSampleData_ParameterisedReport()
        {
            var result = new List<ParameterisedReportModel>
            {
                new ParameterisedReportModel
                {
                    CaseId = 1,
                    CaseNumber = "CN1",
                    AppealNumber = 1,
                    CaseFiledOnYear = 2020,
                    LawyerFullname = "ABC",
                    LocationName = "Delhi",
                    CourtName = "Supreme Court",
                    ProceedingDate = DateTime.Today,
                    NextHearingOn = DateTime.Today.AddDays(1),
                    ProceedingDecision = "PENDING"
                }
            };
            return result;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task GetPendingDisposedCount_Valid(bool isManager)
        {
            // Arrange
            int currUser = 1;
            int userId = isManager ? 0 : currUser;
            PendingDisposedCountModel expected = GetSampleData_PendingDisposedCount();
            _mockSessionService.IsInRoles(default, "Manager").ReturnsForAnyArgs(isManager);
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockInsightsService.GetPendingDisposedCountAsync(userId).Returns(expected);

            // Act
            ActionResult<PendingDisposedCountModel> response = await _sut.GetPendingDisposedCount();

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).IsInRoles(default, "Manager");
            if (isManager)
            {
                _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            }
            else
            {
                _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            }
            await _mockInsightsService.Received(1).GetPendingDisposedCountAsync(userId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            PendingDisposedCountModel actual = (PendingDisposedCountModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.DisposedOff, actual.DisposedOff);
            Assert.Equal(expected.Pending, actual.Pending);
        }

        [Fact]
        public async Task GetParametrisedReport_Valid()
        {
            // Arrange
            ReportFilterModel filterModel = new ReportFilterModel
            {
                CaseNumber = "-1",
                CourtId = -1,
                LawyerId = -1,
                LocationId = -1,
                ProceedingDateRangeStart = DateTime.Today,
                ProceedingDateRangeEnd = DateTime.Today.AddDays(10)
            };
            IEnumerable<ParameterisedReportModel> expected = GetSampleData_ParameterisedReport();
            _mockInsightsService.GetParameterisedReportAsync(default).ReturnsForAnyArgs(expected);

            // Act
            ActionResult<IEnumerable<ParameterisedReportModel>> response = await _sut.GetParametrisedReport(filterModel);

            // Assert
            await _mockInsightsService.Received(1).GetParameterisedReportAsync(Arg.Is<ReportFilterModel>(p
                => p.CaseNumber == filterModel.CaseNumber && p.CourtId == filterModel.CourtId && p.LawyerId == filterModel.LawyerId
                && p.LocationId  == filterModel.LocationId && p.ProceedingDateRangeStart == filterModel.ProceedingDateRangeStart
                && p.ProceedingDateRangeEnd == filterModel.ProceedingDateRangeEnd));
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            IEnumerable<ParameterisedReportModel> actual = (IEnumerable<ParameterisedReportModel>)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseId, actual.ElementAt(i).CaseId);
                Assert.Equal(expected.ElementAt(i).CaseNumber, actual.ElementAt(i).CaseNumber);
                Assert.Equal(expected.ElementAt(i).AppealNumber, actual.ElementAt(i).AppealNumber);
                Assert.Equal(expected.ElementAt(i).CaseFiledOnYear, actual.ElementAt(i).CaseFiledOnYear);
                Assert.Equal(expected.ElementAt(i).LawyerFullname, actual.ElementAt(i).LawyerFullname);
                Assert.Equal(expected.ElementAt(i).LocationName, actual.ElementAt(i).LocationName);
                Assert.Equal(expected.ElementAt(i).CourtName, actual.ElementAt(i).CourtName);
                Assert.Equal(expected.ElementAt(i).ProceedingDate, actual.ElementAt(i).ProceedingDate);
                Assert.Equal(expected.ElementAt(i).NextHearingOn, actual.ElementAt(i).NextHearingOn);
                Assert.Equal(expected.ElementAt(i).ProceedingDecision, actual.ElementAt(i).ProceedingDecision);
            }
        }
    }
}
