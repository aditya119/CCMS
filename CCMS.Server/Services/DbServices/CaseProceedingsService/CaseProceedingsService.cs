using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.CaseProceedingModels;
using Dapper.Oracle;
using System;
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

        public async Task<IEnumerable<CaseProceedingModel>> RetrieveAllCaseProceedingsAsync(int caseId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CaseProceedingModel>("pkg_case_proceedings.p_get_all_case_proceedings", parameters);
        }

        public async Task<CaseProceedingModel> RetrieveAsync(int caseProceedingId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseProceedingModel>("pkg_case_proceedings.p_get_proceeding_details", parameters);
        }

        public async Task UpdateAsync(UpdateCaseProceedingModel caseProceedingModel, int currUser)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_proceeding_id", caseProceedingModel.CaseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_proceeding_date", caseProceedingModel.ProceedingDate, dbType: OracleMappingType.Date, ParameterDirection.Input);
            parameters.Add("pi_proceeding_decision", caseProceedingModel.ProceedingDecision, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_next_hearing_on", caseProceedingModel.NextHearingOn, dbType: OracleMappingType.Date, ParameterDirection.Input);
            parameters.Add("pi_judgement_file", caseProceedingModel.JudgementFile, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_case_proceedings.p_update_case_proceeding", parameters);
        }

        public async Task AssignProceedingAsync(int caseProceedingId, int assignTo, int currUser)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_proceeding_id", caseProceedingId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_assign_to", assignTo, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_case_proceedings.p_assign_proceeding", parameters);
        }
    }
}
