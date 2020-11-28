using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models;
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
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_auth.p_get_auth_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_email", userEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_user_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            sqlModel.Parameters.Add("po_password", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            sqlModel.Parameters.Add("po_salt", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            await _dataAccess.ExecuteAsync(sqlModel);

            int userId = (int)sqlModel.Parameters.Get<decimal>("po_user_id");
            string password = sqlModel.Parameters.Get<string>("po_password");
            string salt = sqlModel.Parameters.Get<string>("po_salt");
            
            return (userId, password, salt);
        }

        public async Task<string> LoginUserAsync(SessionModel sessionModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_auth.p_login",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", sessionModel.UserId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_platform_id", sessionModel.PlatformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_guid", sessionModel.Guid, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            sqlModel.Parameters.Add("po_roles", dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Output, size: 4000);
            await _dataAccess.ExecuteAsync(sqlModel);

            return sqlModel.Parameters.Get<string>("po_roles");
        }

        public async Task LogoutAsync(int userId, int platformId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_auth.p_logout",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_platform_id", platformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task<bool> IsValidSessionAsync(SessionModel sessionModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_auth.p_is_valid_session",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", sessionModel.UserId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_platform_id", sessionModel.PlatformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_guid", sessionModel.Guid, dbType: OracleMappingType.Varchar2, direction: ParameterDirection.Input);
            sqlModel.Parameters.Add("po_is_valid", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            await _dataAccess.ExecuteAsync(sqlModel);

            return (int)sqlModel.Parameters.Get<decimal>("po_is_valid") == 1;
        }
    }
}
