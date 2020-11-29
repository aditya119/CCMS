using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.LocationModels;
using Dapper.Oracle;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class LocationsServiceTests
    {
        private readonly LocationsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public LocationsServiceTests()
        {
            _sut = new LocationsService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewLocationModel locationModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_create_new_location",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_name", locationModel.LocationName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_location_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            NewLocationModel locationModel = new()
            {
                LocationName = "ABC"
            };
            SqlParamsModel queryParams = GetParams_CreateAsync(locationModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(locationModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static IEnumerable<LocationDetailsModel> GetSampleData()
        {
            var result = new List<LocationDetailsModel>
            {
                new LocationDetailsModel { LocationId = 1, LocationName = "ABC" },
                new LocationDetailsModel { LocationId = 2, LocationName = "DEF" }
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_get_all_locations",
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
            IEnumerable<LocationDetailsModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<LocationDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<LocationDetailsModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<LocationDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).LocationId, actual.ElementAt(i).LocationId);
                Assert.Equal(expected.ElementAt(i).LocationName, actual.ElementAt(i).LocationName);
            }
        }

        private static SqlParamsModel GetParams_RetrieveAsync(int locationId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_get_location_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_id", locationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int locationId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(locationId);
            LocationDetailsModel expected = GetSampleData().FirstOrDefault(x => x.LocationId == 1);
            _mockDataAccess.QueryFirstOrDefaultAsync<LocationDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            LocationDetailsModel actual = await _sut.RetrieveAsync(locationId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<LocationDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.LocationId, actual.LocationId);
            Assert.Equal(expected.LocationName, actual.LocationName);
        }

        private static SqlParamsModel GetParams_UpdateAsync(LocationDetailsModel locationModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_update_location",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_id", locationModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_location_name", locationModel.LocationName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            LocationDetailsModel locationModel = GetSampleData().FirstOrDefault();
            SqlParamsModel queryParams = GetParams_UpdateAsync(locationModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(locationModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int locationId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_delete_location",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_id", locationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int locationId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(locationId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(locationId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
