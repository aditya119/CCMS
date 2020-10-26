using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class ProceedingDecisionsService : IProceedingDecisionsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public ProceedingDecisionsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<ProceedingDecisionModel>> RetrieveAllAsync()
        {
            var sqlModel = new ExecuteSqlModel
            {
                Sql = "pkg_proceeding_decisions.p_get_all_proceeding_decisions",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<ProceedingDecisionModel>(sqlModel);
        }

        public async Task<ProceedingDecisionModel> RetrieveAsync(int proceedingDecisionId)
        {
            var sqlModel = new ExecuteSqlModel
            {
                Sql = "pkg_proceeding_decisions.p_get_proceeding_decision_details",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("pi_proceeding_decision_id", proceedingDecisionId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<ProceedingDecisionModel>(sqlModel);
        }
    }
}
