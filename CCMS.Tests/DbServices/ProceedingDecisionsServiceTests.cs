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
    public class ProceedingDecisionsServiceTests
    {
        private readonly ProceedingDecisionsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public ProceedingDecisionsServiceTests()
        {
            _sut = new ProceedingDecisionsService(_mockDataAccess);
        }

        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams_RetrieveAllAsync();
            IEnumerable<ProceedingDecisionModel> expected = GetSampleData();
            _mockDataAccess.QueryAsync<ProceedingDecisionModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<ProceedingDecisionModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<ProceedingDecisionModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).ProceedingDecisionId, actual.ElementAt(i).ProceedingDecisionId);
                Assert.Equal(expected.ElementAt(i).ProceedingDecisionName, actual.ElementAt(i).ProceedingDecisionName);
                Assert.Equal(expected.ElementAt(i).HasNextHearingDate, actual.ElementAt(i).HasNextHearingDate);
                Assert.Equal(expected.ElementAt(i).HasOrderAttachment, actual.ElementAt(i).HasOrderAttachment);
            }
        }

        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int id = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(id);
            ProceedingDecisionModel expected = GetSampleData().FirstOrDefault();
            _mockDataAccess.QueryFirstOrDefaultAsync<ProceedingDecisionModel>(default).ReturnsForAnyArgs(expected);

            // Act
            ProceedingDecisionModel actual = await _sut.RetrieveAsync(id);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<ProceedingDecisionModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.ProceedingDecisionId, actual.ProceedingDecisionId);
            Assert.Equal(expected.ProceedingDecisionName, actual.ProceedingDecisionName);
            Assert.Equal(expected.HasNextHearingDate, actual.HasNextHearingDate);
            Assert.Equal(expected.HasOrderAttachment, actual.HasOrderAttachment);
        }

        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_proceeding_decisions.p_get_all_proceeding_decisions",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }

        private static SqlParamsModel GetParams_RetrieveAsync(int id)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_proceeding_decisions.p_get_proceeding_decision_details",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("pi_proceeding_decision_id", id, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return sqlModel;
        }

        private static IEnumerable<ProceedingDecisionModel> GetSampleData()
        {
            var result = new List<ProceedingDecisionModel>
            {
                new ProceedingDecisionModel { ProceedingDecisionId = 1, ProceedingDecisionName = "PENDING", HasNextHearingDate = false, HasOrderAttachment = false },
                new ProceedingDecisionModel { ProceedingDecisionId = 2, ProceedingDecisionName = "ADJOURNMENT", HasNextHearingDate = true, HasOrderAttachment = false }
            };
            return result;
        }
    }
}
