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
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<ProceedingDecisionModel>("pkg_proceeding_decisions.p_get_all_proceeding_decisions", parameters);
        }
    }
}
