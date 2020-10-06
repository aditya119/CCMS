using System;
using System.Data;
using System.Threading.Tasks;
using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models.CourtCaseModels;
using Dapper.Oracle;

namespace CCMS.Server.DbServices
{
    public class CourtCasesService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CourtCasesService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<(int, int)> SearchCaseNumberAsync(string caseNumber)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_number", caseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_case_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            parameters.Add("po_appeal_number", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_court_cases.p_exists_case_number", parameters);

            int? caseId = (int)parameters.Get<decimal>("po_case_id");
            if (caseId.HasValue == false)
            {
                return (0, 0);
            }
            int appealNumber = (int)parameters.Get<decimal>("po_appeal_number");
            return (caseId.Value, appealNumber);
        }

        public async Task<(string, int, DateTime?)> ExistsCaseIdAsync(int caseId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_case_number", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            parameters.Add("po_appeal_number", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            parameters.Add("po_deleted", dbType: OracleMappingType.Date, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_court_cases.p_exists_case_id", parameters);

            string caseNumber = parameters.Get<string>("po_case_number");
            if (string.IsNullOrEmpty(caseNumber))
            {
                return (string.Empty, 0, null);
            }
            int appealNumber = (int)parameters.Get<decimal>("po_appeal_number");
            DateTime? deleted = parameters.Get<DateTime>("po_deleted");
            return (caseNumber, appealNumber, deleted);
        }

        public async Task<int> CreateAsync(NewCaseModel caseModel, int currUser)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_number", caseModel.CaseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_case_type_id", caseModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_court_id", caseModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_location_id", caseModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_lawyer_id", caseModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_action_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_case_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_court_cases.p_add_new_case", parameters);

            int caseId = (int)parameters.Get<decimal>("po_case_id");
            return caseId;
        }

        public async Task<CaseDetailsModel> RetrieveAsync(int caseId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseDetailsModel>("pkg_court_cases.p_get_case_details", parameters);
        }

        public async Task UpdateAsync(UpdateCaseModel caseModel, int currUser)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_id", caseModel.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_case_number", caseModel.CaseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_case_type_id", caseModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_court_id", caseModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_location_id", caseModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_lawyer_id", caseModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_court_cases.p_update_case", parameters);
        }

        public async Task DeleteAsync(int caseId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_court_cases.p_delete_case", parameters);
        }
    }
}
