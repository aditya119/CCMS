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
    public class RolesServiceTests
    {
        private readonly RolesService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public RolesServiceTests()
        {
            _sut = new RolesService(_mockDataAccess);
        }

        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams_RetrieveAllAsync();
            IEnumerable<RoleModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<RoleModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<RoleModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<RoleModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).RoleId, actual.ElementAt(i).RoleId);
                Assert.Equal(expected.ElementAt(i).RoleName, actual.ElementAt(i).RoleName);
                Assert.Equal(expected.ElementAt(i).RoleDescription, actual.ElementAt(i).RoleDescription);
            }
        }

        [Fact]
        public async Task GetRoleIdAsync_Valid()
        {
            // Arrange
            string rolesCsv = "Operator,Manager";
            SqlParamsModel queryParams = GetParams_GetRoleIdAsync(rolesCsv);
            //int expected = 3;
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(0);

            // Act
            await _sut.GetRoleIdAsync(rolesCsv);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            // Assert.Equal(expected, actual);
        }

        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_roles.p_get_all_roles",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }

        private static SqlParamsModel GetParams_GetRoleIdAsync(string rolesCsv)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_roles.p_get_role_id",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_roles_csv", rolesCsv, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_role_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            
            return sqlModel;
        }

        private static IEnumerable<RoleModel> GetSampleData()
        {
            var result = new List<RoleModel>
            {
                new RoleModel { RoleId = 1, RoleName = "Operator", RoleDescription = "TBD" },
                new RoleModel { RoleId = 2, RoleName = "Manager", RoleDescription = "TBD" }
            };
            return result;
        }
    }
}
