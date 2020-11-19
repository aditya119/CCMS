using CCMS.Server.DbDataAccess;
using CCMS.Server.DbServices;
using CCMS.Shared.Models;
using Dapper.Oracle;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class PlatformsServiceTests
    {
        private readonly PlatformsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public PlatformsServiceTests()
        {
            _sut = new PlatformsService(_mockDataAccess);
        }

        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams();
            IEnumerable<PlatformModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<PlatformModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<PlatformModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<PlatformModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).PlatformId, actual.ElementAt(i).PlatformId);
                Assert.Equal(expected.ElementAt(i).PlatformName, actual.ElementAt(i).PlatformName);
            }
        }

        private static SqlParamsModel GetParams()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_platforms.p_get_all_platforms",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }

        private static IEnumerable<PlatformModel> GetSampleData()
        {
            var result = new List<PlatformModel>
            {
                new PlatformModel { PlatformId = 1, PlatformName = "MOBILE" },
                new PlatformModel { PlatformId = 2, PlatformName = "DESKTOP" }
            };
            return result;
        }
    }
}
