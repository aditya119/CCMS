using CCMS.Server.DbDataAccess;
using Dapper.Oracle;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class AuthService : IAuthService
    {
        private readonly IOracleDataAccess _dataAccess;

        public AuthService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<(int, string, string)> FetchUserDetailsAsync(string userEmail)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_user_email", userEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_user_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            parameters.Add("po_password", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            parameters.Add("po_salt", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            await _dataAccess.ExecuteAsync("pkg_auth.p_get_auth_details", parameters);

            int userId = (int)parameters.Get<decimal>("po_user_id");
            string password = parameters.Get<string>("po_password");
            string salt = parameters.Get<string>("po_salt");
            
            return (userId, password, salt);
        }

        public async Task<string> LoginUserAsync(int userId, int platformId, string guid)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_platform_id", platformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            parameters.Add("pi_guid", guid, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            parameters.Add("po_roles", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            await _dataAccess.ExecuteAsync("pkg_auth.p_login", parameters);

            return parameters.Get<string>("po_roles");
        }
    }
}
