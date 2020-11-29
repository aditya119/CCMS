using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.CourtModels;
using Dapper.Oracle;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class CourtsServiceTests
    {
        private readonly CourtsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public CourtsServiceTests()
        {
            _sut = new CourtsService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewCourtModel courtModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_create_new_court",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_name", courtModel.CourtName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_court_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            NewCourtModel courtModel = new()
            {
                CourtName = "ABC"
            };
            SqlParamsModel queryParams = GetParams_CreateAsync(courtModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(courtModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static IEnumerable<CourtDetailsModel> GetSampleData()
        {
            var result = new List<CourtDetailsModel>
            {
                new CourtDetailsModel { CourtId = 1, CourtName = "Supreme Court" },
                new CourtDetailsModel { CourtId = 2, CourtName = "High Court" }
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_get_all_courts",
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
            IEnumerable<CourtDetailsModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<CourtDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<CourtDetailsModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<CourtDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CourtId, actual.ElementAt(i).CourtId);
                Assert.Equal(expected.ElementAt(i).CourtName, actual.ElementAt(i).CourtName);
            }
        }

        private static SqlParamsModel GetParams_RetrieveAsync(int courtId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_get_court_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_id", courtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int courtId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(courtId);
            CourtDetailsModel expected = GetSampleData().FirstOrDefault(x => x.CourtId == 1);
            _mockDataAccess.QueryFirstOrDefaultAsync<CourtDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            CourtDetailsModel actual = await _sut.RetrieveAsync(courtId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<CourtDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.CourtId, actual.CourtId);
            Assert.Equal(expected.CourtName, actual.CourtName);
        }

        private static SqlParamsModel GetParams_UpdateAsync(CourtDetailsModel courtModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_update_court",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_id", courtModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_court_name", courtModel.CourtName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            CourtDetailsModel courtModel = GetSampleData().FirstOrDefault();
            SqlParamsModel queryParams = GetParams_UpdateAsync(courtModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(courtModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int courtId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_delete_court",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_id", courtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int courtId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(courtId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(courtId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
