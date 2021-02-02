using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
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
    public class CaseProceedingsServiceTests
    {
        private readonly CaseProceedingsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public CaseProceedingsServiceTests()
        {
            _sut = new CaseProceedingsService(_mockDataAccess);
        }

        private static IEnumerable<PendingProceedingModel> GetSampleData_RetrievePendingProceedingsAsync()
        {
            var result = new List<PendingProceedingModel>
            {
                new PendingProceedingModel { CaseProceedingId = 1, CaseNumber = "CN1", AppealNumber = 1, ProceedingDate = DateTime.Today.AddDays(-1), NextHearingOn = DateTime.Today, CaseStatus = "PENDING", AssignedTo = "Abc (abc@xyz.com)" },
                new PendingProceedingModel { CaseProceedingId = 2, CaseNumber = "CN2", AppealNumber = 0, ProceedingDate = DateTime.Today.AddDays(-1), NextHearingOn = DateTime.Today, CaseStatus = "PENDING", AssignedTo = "Abc (abc@xyz.com)" },
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrievePendingProceedingsAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_get_pending_proceedings",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrievePendingProceedingsAsync_Valid()
        {
            // Arrange
            int userId = 1;
            SqlParamsModel queryParams = GetParams_RetrievePendingProceedingsAsync(userId);
            IEnumerable<PendingProceedingModel> expected = GetSampleData_RetrievePendingProceedingsAsync();
            _mockDataAccess.QueryAsync<PendingProceedingModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<PendingProceedingModel> actual = await _sut.RetrievePendingProceedingsAsync(userId);

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<PendingProceedingModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
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

        private static IEnumerable<CaseProceedingModel> GetSampleData()
        {
            var result = new List<CaseProceedingModel>
            {
                new CaseProceedingModel { CaseProceedingId = 1, ProceedingDate = DateTime.Today.AddDays(-1), NextHearingOn = DateTime.Today, ProceedingDecision = 1, JudgementFile = 0 },
                new CaseProceedingModel { CaseProceedingId = 2, ProceedingDate = DateTime.Today, NextHearingOn = DateTime.Today.AddDays(1), ProceedingDecision = 0, JudgementFile = 0 }
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAllCaseProceedingsAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_get_all_case_proceedings",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAllCaseProceedingsAsync_Valid()
        {
            // Arrange
            int caseId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAllCaseProceedingsAsync(caseId);
            IEnumerable<CaseProceedingModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<CaseProceedingModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<CaseProceedingModel> actual = await _sut.RetrieveAllCaseProceedingsAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<CaseProceedingModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
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

        private static SqlParamsModel GetParams_RetrieveAsync(int caseProceedingId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_get_proceeding_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int caseProceedingId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(caseProceedingId);
            CaseProceedingModel expected = GetSampleData().FirstOrDefault(x => x.CaseProceedingId == caseProceedingId);
            _mockDataAccess.QueryFirstOrDefaultAsync<CaseProceedingModel>(default).ReturnsForAnyArgs(expected);

            // Act
            CaseProceedingModel actual = await _sut.RetrieveAsync(caseProceedingId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<CaseProceedingModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseProceedingId, actual.CaseProceedingId);
            Assert.Equal(expected.ProceedingDate, actual.ProceedingDate);
            Assert.Equal(expected.NextHearingOn, actual.NextHearingOn);
            Assert.Equal(expected.ProceedingDecision, actual.ProceedingDecision);
            Assert.Equal(expected.JudgementFile, actual.JudgementFile);
        }

        private static SqlParamsModel GetParams_UpdateAsync(CaseProceedingModel caseProceedingModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_update_case_proceeding",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingModel.CaseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_proceeding_date", caseProceedingModel.ProceedingDate, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_proceeding_decision", caseProceedingModel.ProceedingDecision, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_next_hearing_on", caseProceedingModel.NextHearingOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_judgement_file", caseProceedingModel.JudgementFile, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            var caseProceedingModel = GetSampleData().FirstOrDefault(x => x.CaseProceedingId == 1);
            int currUser = 1;
            SqlParamsModel queryParams = GetParams_UpdateAsync(caseProceedingModel, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(caseProceedingModel, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_AssignProceedingAsync(int caseProceedingId, int assignTo, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_assign_proceeding",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_assign_to", assignTo, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task AssignProceedingAsync_Valid()
        {
            // Arrange
            int caseProceedingId = 1;
            int assignTo = 2;
            int currUser = 1;
            SqlParamsModel queryParams = GetParams_AssignProceedingAsync(caseProceedingId, assignTo, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.AssignProceedingAsync(caseProceedingId, assignTo, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int caseProceedingId, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_delete_case_proceeding",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int caseProceedingId = 1;
            int currUser = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(caseProceedingId, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(caseProceedingId, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
