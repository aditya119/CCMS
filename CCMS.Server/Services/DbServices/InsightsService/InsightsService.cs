using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.InsightsModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class InsightsService : IInsightsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public InsightsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<PendingDisposedCountModel> GetPendingDisposedCountAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_insights.p_pending_disposed_cases",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<PendingDisposedCountModel>(sqlModel);
        }

        public async Task<IEnumerable<ParameterisedReportModel>> GetParameterisedReportAsync(ReportFilterModel filterModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_insights.p_parameterised_report",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_csv_filter_params", filterModel.Csv, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<ParameterisedReportModel>(sqlModel);
        }
    }
}
