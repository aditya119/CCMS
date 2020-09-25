using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models.CaseTypeModels;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class CaseTypesService : ICaseTypesService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CaseTypesService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewCaseTypeModel caseTypeModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_type_name", caseTypeModel.CaseTypeName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_case_type_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_case_types.p_create_new_case_type", parameters);

            int case_typeId = (int)parameters.Get<decimal>("po_case_type_id");
            return case_typeId;
        }

        public async Task<IEnumerable<CaseTypeDetailsModel>> RetrieveAllAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CaseTypeDetailsModel>("pkg_case_types.p_get_all_case_types", parameters);
        }

        public async Task<CaseTypeDetailsModel> RetrieveAsync(int case_typeId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_type_id", case_typeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseTypeDetailsModel>("pkg_case_types.p_get_case_type_details", parameters);
        }

        public async Task UpdateAsync(CaseTypeDetailsModel caseTypeModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_type_id", caseTypeModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_case_type_name", caseTypeModel.CaseTypeName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_case_types.p_update_case_type", parameters);
        }

        public async Task DeleteAsync(int case_typeId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_case_type_id", case_typeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_case_types.p_delete_case_type", parameters);
        }
    }
}
