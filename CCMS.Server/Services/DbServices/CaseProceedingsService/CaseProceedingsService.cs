using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class CaseProceedingsService : ICaseProceedingsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CaseProceedingsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<AssignedProceedingModel>> RetrieveAssignedProceedingsAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_get_assigned_proceedings",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<AssignedProceedingModel>(sqlModel);
        }

        public async Task<IEnumerable<CaseProceedingModel>> RetrieveAllCaseProceedingsAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_get_all_case_proceedings",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CaseProceedingModel>(sqlModel);
        }

        public async Task<CaseProceedingModel> RetrieveAsync(int caseProceedingId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_get_proceeding_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseProceedingModel>(sqlModel);
        }

        public async Task UpdateAsync(CaseProceedingModel caseProceedingModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_update_case_proceeding",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingModel.CaseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_proceeding_date", caseProceedingModel.ProceedingDate, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_proceeding_decision", caseProceedingModel.ProceedingDecision, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_next_hearing_on", caseProceedingModel.NextHearingOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_judgement_file", caseProceedingModel.JudgementFile, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task AssignProceedingAsync(int caseProceedingId, int assignTo, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_assign_proceeding",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_assign_to", assignTo, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int caseProceedingId, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_proceedings.p_delete_case_proceeding",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
