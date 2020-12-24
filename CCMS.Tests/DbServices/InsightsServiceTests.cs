using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.InsightsModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class InsightsServiceTests
    {
        private readonly InsightsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public InsightsServiceTests()
        {
            _sut = new InsightsService(_mockDataAccess);
        }

        private static PendingDisposedCountModel GetSampleData_GetPendingDisposedCountAsync()
        {
            return new PendingDisposedCountModel
            {
                DisposedOff = 1,
                Pending = 3
            };
        }
        private static SqlParamsModel GetParams_GetPendingDisposedCountAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_insights.p_pending_disposed_cases",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task GetPendingDisposedCountAsync_Valid()
        {
            // Arrange
            int userId = 1;
            SqlParamsModel queryParams = GetParams_GetPendingDisposedCountAsync(userId);
            PendingDisposedCountModel expected = GetSampleData_GetPendingDisposedCountAsync();
            _mockDataAccess.QueryFirstOrDefaultAsync<PendingDisposedCountModel>(default).ReturnsForAnyArgs(expected);

            // Act
            PendingDisposedCountModel actual = await _sut.GetPendingDisposedCountAsync(userId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<PendingDisposedCountModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.DisposedOff, actual.DisposedOff);
            Assert.Equal(expected.Pending, actual.Pending);
        }

        private static IEnumerable<ParameterisedReportModel> GetSampleData_GetParameterisedReportAsync()
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
        private static SqlParamsModel GetParams_GetParameterisedReportAsync(ReportFilterModel filterModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_insights.p_parameterised_report",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_csv_filter_params", filterModel.Csv, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task GetParameterisedReportAsync_Valid()
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
            SqlParamsModel queryParams = GetParams_GetParameterisedReportAsync(filterModel);
            IEnumerable<ParameterisedReportModel> expected = GetSampleData_GetParameterisedReportAsync();
            _mockDataAccess.QueryAsync<ParameterisedReportModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<ParameterisedReportModel> actual = await _sut.GetParameterisedReportAsync(filterModel);

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<ParameterisedReportModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
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
