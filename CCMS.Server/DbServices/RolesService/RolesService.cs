using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class RolesService : IRolesService
    {
        private readonly IOracleDataAccess _dataAccess;

        public RolesService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> GetRoleIdAsync(string rolesCsv)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_roles.p_get_role_id",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_roles_csv", rolesCsv, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_role_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            
            await _dataAccess.ExecuteAsync(sqlModel);

            int roleId = (int)sqlModel.Parameters.Get<decimal>("po_role_id");
            return roleId;
        }

        public async Task<IEnumerable<RoleModel>> RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_roles.p_get_all_roles",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<RoleModel>(sqlModel);
        }
    }
}
