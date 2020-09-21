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

        public async Task<int> GetRoleId(string rolesCsv)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_roles_csv", rolesCsv, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_role_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_roles.p_get_role_id", parameters);

            int roleId = (int)parameters.Get<decimal>("po_role_id");
            return roleId;
        }

        public async Task<IEnumerable<RoleModel>> GetAllRoles()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<RoleModel>("pkg_roles.p_get_all_roles", parameters);
        }
    }
}
