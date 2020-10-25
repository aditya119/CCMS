using CCMS.Server.DbDataAccess;
using CCMS.Server.DbServices;
using CCMS.Shared.Models;
using Dapper.Oracle;
using Moq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class ActorTypesServiceTests
    {
        private readonly ActorTypesService _sut;
        private readonly Mock<IOracleDataAccess> _mockDataAccess = new Mock<IOracleDataAccess>();
        public ActorTypesServiceTests()
        {
            _sut = new ActorTypesService(_mockDataAccess.Object);
        }

        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            ExecuteSqlModel queryParams = GetParams();
            IEnumerable<ActorTypeModel> expected = GetSampleData();
            _mockDataAccess.Setup(x => x.QueryAsync<ActorTypeModel>(queryParams))
                .ReturnsAsync(expected);

            // Act
            IEnumerable<ActorTypeModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            _mockDataAccess.Verify(x => x.QueryAsync<ActorTypeModel>(queryParams), Times.Once);
            Assert.True(actual != null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).ActorTypeId, actual.ElementAt(i).ActorTypeId);
                Assert.Equal(expected.ElementAt(i).ActorTypeName, actual.ElementAt(i).ActorTypeName);
            }
        }

        private ExecuteSqlModel GetParams()
        {
            var sqlModel = new ExecuteSqlModel
            {
                Sql = "pkg_actor_types.p_get_all_actor_types",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }

        private IEnumerable<ActorTypeModel> GetSampleData()
        {
            var result = new List<ActorTypeModel>
            {
                new ActorTypeModel { ActorTypeId = 1, ActorTypeName = "PETITIONER" },
                new ActorTypeModel { ActorTypeId = 2, ActorTypeName = "RESPONDENT" }
            };
            return result;
        }
    }
}
