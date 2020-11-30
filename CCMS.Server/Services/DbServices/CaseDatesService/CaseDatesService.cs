using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models;
using Dapper.Oracle;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class CaseDatesService : ICaseDatesService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CaseDatesService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<CaseDatesModel> RetrieveAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_dates.p_get_case_dates",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseDatesModel>(sqlModel);
        }

        public async Task UpdateAsync(CaseDatesModel caseDatesModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_dates.p_update_case_dates",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseDatesModel.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_filed_on", caseDatesModel.CaseFiledOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_notice_received_on", caseDatesModel.NoticeReceivedOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_first_hearing_on", caseDatesModel.FirstHearingOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
