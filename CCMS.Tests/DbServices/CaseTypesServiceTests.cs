using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.CaseTypeModels;
using Dapper.Oracle;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class CaseTypesServiceTests
    {
        private readonly CaseTypesService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public CaseTypesServiceTests()
        {
            _sut = new CaseTypesService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewCaseTypeModel caseTypeModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_create_new_case_type",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_name", caseTypeModel.CaseTypeName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_case_type_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            NewCaseTypeModel caseTypeModel = new()
            {
                CaseTypeName = "ABC"
            };
            SqlParamsModel queryParams = GetParams_CreateAsync(caseTypeModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(caseTypeModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static IEnumerable<CaseTypeDetailsModel> GetSampleData()
        {
            var result = new List<CaseTypeDetailsModel>
            {
                new CaseTypeDetailsModel { CaseTypeId = 1, CaseTypeName = "Civil" },
                new CaseTypeDetailsModel { CaseTypeId = 2, CaseTypeName = "Criminal" }
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_get_all_case_types",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams_RetrieveAllAsync();
            IEnumerable<CaseTypeDetailsModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<CaseTypeDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<CaseTypeDetailsModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<CaseTypeDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseTypeId, actual.ElementAt(i).CaseTypeId);
                Assert.Equal(expected.ElementAt(i).CaseTypeName, actual.ElementAt(i).CaseTypeName);
            }
        }

        private static SqlParamsModel GetParams_RetrieveAsync(int caseTypeId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_get_case_type_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_id", caseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int caseTypeId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(caseTypeId);
            CaseTypeDetailsModel expected = GetSampleData().FirstOrDefault(x => x.CaseTypeId == 1);
            _mockDataAccess.QueryFirstOrDefaultAsync<CaseTypeDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            CaseTypeDetailsModel actual = await _sut.RetrieveAsync(caseTypeId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<CaseTypeDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.CaseTypeId, actual.CaseTypeId);
            Assert.Equal(expected.CaseTypeName, actual.CaseTypeName);
        }

        private static SqlParamsModel GetParams_UpdateAsync(CaseTypeDetailsModel caseTypeModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_update_case_type",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_id", caseTypeModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_type_name", caseTypeModel.CaseTypeName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            CaseTypeDetailsModel caseTypeModel = GetSampleData().FirstOrDefault();
            SqlParamsModel queryParams = GetParams_UpdateAsync(caseTypeModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(caseTypeModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int caseTypeId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_delete_case_type",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_id", caseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int caseTypeId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(caseTypeId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(caseTypeId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
