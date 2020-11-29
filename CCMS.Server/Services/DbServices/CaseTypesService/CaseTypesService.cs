using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.CaseTypeModels;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
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
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_create_new_case_type",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_name", caseTypeModel.CaseTypeName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_case_type_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            int case_typeId = (int)sqlModel.Parameters.Get<decimal>("po_case_type_id");
            return case_typeId;
        }

        public async Task<IEnumerable<CaseTypeDetailsModel>> RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_get_all_case_types",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CaseTypeDetailsModel>(sqlModel);
        }

        public async Task<CaseTypeDetailsModel> RetrieveAsync(int caseTypeId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_get_case_type_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_id", caseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CaseTypeDetailsModel>(sqlModel);
        }

        public async Task UpdateAsync(CaseTypeDetailsModel caseTypeModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_update_case_type",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_id", caseTypeModel.CaseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_case_type_name", caseTypeModel.CaseTypeName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int caseTypeId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_case_types.p_delete_case_type",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_case_type_id", caseTypeId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
