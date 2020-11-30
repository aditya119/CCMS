using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
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
    public class CaseActorsServiceTests
    {
        private readonly CaseActorsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public CaseActorsServiceTests()
        {
            _sut = new CaseActorsService(_mockDataAccess);
        }

        private static IEnumerable<CaseActorModel> GetSampleData(int caseId)
        {
            var result = new List<CaseActorModel>
            {
                new CaseActorModel { CaseId = caseId, ActorTypeId = 1, ActorName = "Abc", ActorAddress = "xyz", ActorEmail = "abc@xyz.com", ActorPhone = "12", DetailFile = 0 },
                new CaseActorModel { CaseId = caseId, ActorTypeId = 2, ActorName = "Defc", ActorAddress = "xyz", ActorEmail = "def@xyz.com", ActorPhone = "234", DetailFile = 0 }
            };
            return result;
        }

        private static SqlParamsModel GetParams_RetrieveAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_actors.p_get_all_case_actors",
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
            IEnumerable<CaseActorModel> expected = GetSampleData(caseId);
            _mockDataAccess.QueryAsync<CaseActorModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<CaseActorModel> actual = await _sut.RetrieveAsync(caseId);

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<CaseActorModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).CaseId, actual.ElementAt(i).CaseId);
                Assert.Equal(expected.ElementAt(i).ActorTypeId, actual.ElementAt(i).ActorTypeId);
                Assert.Equal(expected.ElementAt(i).ActorName, actual.ElementAt(i).ActorName);
                Assert.Equal(expected.ElementAt(i).ActorAddress, actual.ElementAt(i).ActorAddress);
                Assert.Equal(expected.ElementAt(i).ActorEmail, actual.ElementAt(i).ActorEmail);
                Assert.Equal(expected.ElementAt(i).ActorPhone, actual.ElementAt(i).ActorPhone);
            }
        }

        private static List<SqlParamsModel> GetParams_UpdateAsync(IEnumerable<CaseActorModel> caseActorModels, int currUser)
        {
            var executeSqlModels = new List<SqlParamsModel>();
            foreach (var model in caseActorModels)
            {
                var sqlModel = new SqlParamsModel
                {
                    Sql = "pkg_case_actors.p_update_case_actors",
                    Parameters = new OracleDynamicParameters()
                };
                sqlModel.Parameters.Add("pi_case_id", model.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_type_id", model.ActorTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_name", model.ActorName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_address", model.ActorAddress, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_email", model.ActorEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_actor_phone", model.ActorPhone, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_detail_file", model.DetailFile, dbType: OracleMappingType.Int32, ParameterDirection.Input);
                sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

                executeSqlModels.Add(sqlModel);
            }
            return executeSqlModels;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            int currUser = 1;
            int caseId = 1;
            var caseActorModels = GetSampleData(caseId);
            List<SqlParamsModel> queryParams = GetParams_UpdateAsync(caseActorModels, currUser);
            _mockDataAccess.ExecuteTransactionAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(caseActorModels, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteTransactionAsync(Arg.Is<IEnumerable<SqlParamsModel>>(
                p => p.Count() == queryParams.Count
                && EquatableOracleDynamicParameters.AreEqual(p.ElementAt(0).Parameters, queryParams[0].Parameters)
                && EquatableOracleDynamicParameters.AreEqual(p.ElementAt(1).Parameters, queryParams[1].Parameters)
                ));
        }
    }
}
