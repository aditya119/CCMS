using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.CourtCaseModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using NSubstitute;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class CourtCasesServiceTests
    {
        private readonly CourtCasesService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public CourtCasesServiceTests()
        {
            _sut = new CourtCasesService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_ExistsCaseNumberAsync(string caseNumber, int appealNumber)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_exists_case_number",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_number", caseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_appeal_number", appealNumber, dbType: OracleMappingType.Int32, ParameterDirection.Output);
            sqlModel.Parameters.Add("po_case_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            sqlModel.Parameters.Add("po_deleted", dbType: OracleMappingType.Date, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task ExistsCaseNumberAsync_Valid()
        {
            // Arrange
            string caseNumber = "CN1";
            int appealNumber = 0;
            SqlParamsModel queryParams = GetParams_ExistsCaseNumberAsync(caseNumber, appealNumber);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(0);

            // Act
            (int caseId, DateTime? deleted) = await _sut.ExistsCaseNumberAsync(caseNumber, appealNumber);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_ExistsCaseIdAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_exists_case_id",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_case_number", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            sqlModel.Parameters.Add("po_appeal_number", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            sqlModel.Parameters.Add("po_deleted", dbType: OracleMappingType.Date, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task ExistsCaseIdAsync_Valid()
        {
            // Arrange
            int caseId = 1;
            SqlParamsModel queryParams = GetParams_ExistsCaseIdAsync(caseId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(0);

            // Act
            (string caseNumber, int appealNumber, DateTime? deleted) = await _sut.ExistsCaseIdAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static CaseStatusModel GetSampleData_GetCaseStatusAsync()
        {
            var result = new CaseStatusModel
            {
                StatusId = 1,
                StatusName = "FINAL JUDGEMENT"
            };
            return result;
        }
        private static SqlParamsModel GetParams_GetCaseStatusAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_get_case_status",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task GetCaseStatusAsync_Valid()
        {
            // Arrange
            int caseId = 1;
            SqlParamsModel queryParams = GetParams_GetCaseStatusAsync(caseId);
            CaseStatusModel expected = GetSampleData_GetCaseStatusAsync();
            _mockDataAccess.QueryFirstOrDefaultAsync<CaseStatusModel>(default).ReturnsForAnyArgs(expected);

            // Act
            CaseStatusModel actual = await _sut.GetCaseStatusAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<CaseStatusModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.StatusId, actual.StatusId);
            Assert.Equal(expected.StatusName, actual.StatusName);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewCaseModel caseModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_add_new_case",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_number", caseModel.CaseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_appeal_number", caseModel.AppealNumber, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_type_id", caseModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_court_id", caseModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_location_id", caseModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_id", caseModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_action_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_case_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
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
            int currUser = 1;
            SqlParamsModel queryParams = GetParams_CreateAsync(caseModel, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            int caseId = await _sut.CreateAsync(caseModel, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static CaseDetailsModel GetSampleData_RetrieveAsync(int caseId)
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
                CaseStatus = 1,
                Deleted = null
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_get_case_details",
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
            CaseDetailsModel expected = GetSampleData_RetrieveAsync(caseId);
            _mockDataAccess.QueryFirstOrDefaultAsync<CaseDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            CaseDetailsModel actual = await _sut.RetrieveAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<CaseDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseId, actual.CaseId);
            Assert.Equal(expected.CaseNumber, actual.CaseNumber);
            Assert.Equal(expected.AppealNumber, actual.AppealNumber);
            Assert.Equal(expected.CaseStatus, actual.CaseStatus);
            Assert.Equal(expected.CaseTypeId, actual.CaseTypeId);
            Assert.Equal(expected.CourtId, actual.CourtId);
            Assert.Equal(expected.LawyerId, actual.LawyerId);
            Assert.Equal(expected.LocationId, actual.LocationId);
            Assert.Equal(expected.Deleted, actual.Deleted);
        }

        private static SqlParamsModel GetParams_UpdateAsync(UpdateCaseModel caseModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_update_case",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseModel.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_number", caseModel.CaseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_appeal_number", caseModel.AppealNumber, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_type_id", caseModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_court_id", caseModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_location_id", caseModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_id", caseModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
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
            SqlParamsModel queryParams = GetParams_UpdateAsync(caseModel, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(caseModel, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_delete_case",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int caseId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(caseId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
