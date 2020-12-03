using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using CCMS.Server.Models;
using Dapper.Oracle;
using NSubstitute;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class CaseDatesServiceTests
    {
        private readonly CaseDatesService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public CaseDatesServiceTests()
        {
            _sut = new CaseDatesService(_mockDataAccess);
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

        private static SqlParamsModel GetParams_RetrieveAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_dates.p_get_case_dates",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int caseId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(caseId);
            CaseDatesModel expected = GetSampleData(caseId);
            _mockDataAccess.QueryFirstOrDefaultAsync<CaseDatesModel>(default).ReturnsForAnyArgs(expected);

            // Act
            CaseDatesModel actual = await _sut.RetrieveAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<CaseDatesModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseId, actual.CaseId);
            Assert.Equal(expected.CaseFiledOn, actual.CaseFiledOn);
            Assert.Equal(expected.NoticeReceivedOn, actual.NoticeReceivedOn);
            Assert.Equal(expected.FirstHearingOn, actual.FirstHearingOn);
        }

        private static SqlParamsModel GetParams_UpdateAsync(CaseDatesModel caseDatesModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_dates.p_update_case_dates",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseDatesModel.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_filed_on", caseDatesModel.CaseFiledOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_notice_received_on", caseDatesModel.NoticeReceivedOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_first_hearing_on", caseDatesModel.FirstHearingOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            int currUser = 1;
            var caseDatesModel = new CaseDatesModel
            {
                CaseId = 1,
                CaseFiledOn = DateTime.Today,
                NoticeReceivedOn = DateTime.Today,
                FirstHearingOn = DateTime.Today.AddDays(1)
            };
            SqlParamsModel queryParams = GetParams_UpdateAsync(caseDatesModel, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(caseDatesModel, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
