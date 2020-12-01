using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models;
using Dapper.Oracle;
using NSubstitute;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class AuthServiceTests
    {
        private readonly AuthService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public AuthServiceTests()
        {
            _sut = new AuthService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_FetchUserDetailsAsync(string userEmail)
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
            return sqlModel;
        }
        [Fact]
        public async Task FetchUserDetailsAsync_Valid()
        {
            // Arrange
            string userEmail = "abc@xyz.com";
            SqlParamsModel queryParams = GetParams_FetchUserDetailsAsync(userEmail);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.FetchUserDetailsAsync(userEmail);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_IncrementLoginCountAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_auth.p_increment_login_count",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task IncrementLoginCountAsync_Valid()
        {
            // Arrange
            int userId = 1;
            SqlParamsModel queryParams = GetParams_IncrementLoginCountAsync(userId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.IncrementLoginCountAsync(userId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_LoginUserAsync(SessionModel sessionModel)
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
            return sqlModel;
        }
        [Fact]
        public async Task LoginUserAsync_Valid()
        {
            // Arrange
            SessionModel sessionModel = new (1, 1, "abc");
            SqlParamsModel queryParams = GetParams_LoginUserAsync(sessionModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.LoginUserAsync(sessionModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_LogoutAsync(int userId, int platformId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_auth.p_logout",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_platform_id", platformId, dbType: OracleMappingType.Int32, direction: ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task LogoutAsync_Valid()
        {
            // Arrange
            int userId = 1;
            int platformId = 1;
            SqlParamsModel queryParams = GetParams_LogoutAsync(userId, platformId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.LogoutAsync(userId, platformId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_IsValidSessionAsync(SessionModel sessionModel)
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
            return sqlModel;
        }
        [Fact]
        public async Task IsValidSessionAsync_Valid()
        {
            // Arrange
            SessionModel sessionModel = new (1, 1, "abc");
            SqlParamsModel queryParams = GetParams_IsValidSessionAsync(sessionModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.IsValidSessionAsync(sessionModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
