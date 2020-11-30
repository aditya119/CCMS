using System;
using System.Data;
using System.Threading.Tasks;
using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.CourtCaseModels;
using Dapper.Oracle;

namespace CCMS.Server.Services.DbServices
{
    public class CourtCasesService : ICourtCasesService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CourtCasesService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<(int, DateTime?)> ExistsCaseNumberAsync(string caseNumber, int appealNumber)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_exists_case_number",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_number", caseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_appeal_number", appealNumber, dbType: OracleMappingType.Int32, ParameterDirection.Output);
            sqlModel.Parameters.Add("po_case_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            sqlModel.Parameters.Add("po_deleted", dbType: OracleMappingType.Date, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            int caseId = (int)sqlModel.Parameters.Get<decimal>("po_case_id");
            DateTime? deleted = sqlModel.Parameters.Get<DateTime>("po_deleted");
            return (caseId, deleted);
        }

        public async Task<(string, int, DateTime?)> ExistsCaseIdAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_exists_case_id",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_case_number", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            sqlModel.Parameters.Add("po_appeal_number", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            sqlModel.Parameters.Add("po_deleted", dbType: OracleMappingType.Date, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            string caseNumber = sqlModel.Parameters.Get<string>("po_case_number");
            int appealNumber = (int)sqlModel.Parameters.Get<decimal>("po_appeal_number");
            DateTime? deleted = sqlModel.Parameters.Get<DateTime>("po_deleted");
            return (caseNumber, appealNumber, deleted);
        }

        public async Task<CaseStatusModel> GetCaseStatusAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_get_case_status",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseStatusModel>(sqlModel);
        }

        /// <summary>
        /// Add new case, case is restored if deleted
        /// </summary>
        /// <param name="caseModel"></param>
        /// <param name="currUser"></param>
        /// <returns>-1 if undeleted case already exists, new caseId otherwise</returns>
        public async Task<int> CreateAsync(NewCaseModel caseModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_add_new_case",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_number", caseModel.CaseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_appeal_number", caseModel.AppealNumber, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_type_id", caseModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_court_id", caseModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_location_id", caseModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_id", caseModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_action_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_case_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            int caseId = (int)sqlModel.Parameters.Get<decimal>("po_case_id");
            return caseId;
        }

        public async Task<CaseDetailsModel> RetrieveAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_get_case_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseDetailsModel>(sqlModel);
        }

        public async Task UpdateAsync(UpdateCaseModel caseModel, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_update_case",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseModel.CaseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_number", caseModel.CaseNumber, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_appeal_number", caseModel.AppealNumber, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_type_id", caseModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_court_id", caseModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_location_id", caseModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_lawyer_id", caseModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int caseId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_court_cases.p_delete_case",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_id", caseId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
