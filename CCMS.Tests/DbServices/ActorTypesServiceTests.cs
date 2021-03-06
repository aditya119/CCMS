﻿using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using CCMS.Server.Models;
using Dapper.Oracle;
using NSubstitute;
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
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public ActorTypesServiceTests()
        {
            _sut = new ActorTypesService(_mockDataAccess);
        }

        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams();
            IEnumerable<ActorTypeModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<ActorTypeModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<ActorTypeModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<ActorTypeModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).ActorTypeId, actual.ElementAt(i).ActorTypeId);
                Assert.Equal(expected.ElementAt(i).ActorTypeName, actual.ElementAt(i).ActorTypeName);
            }
        }

        private static SqlParamsModel GetParams()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_actor_types.p_get_all_actor_types",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }

        private static IEnumerable<ActorTypeModel> GetSampleData()
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
