using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.AppUserModels;
using Dapper.Oracle;
using NSubstitute;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class AppUsersServiceTests
    {
        private readonly AppUsersService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public AppUsersServiceTests()
        {
            _sut = new AppUsersService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewUserModel userModel, string passwordSalt, string hashedPassword)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_create_new_user",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_email", userModel.UserEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_fullname", userModel.UserFullname, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_password", hashedPassword, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_password_salt", passwordSalt, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_roles", userModel.UserRoles, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_user_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            string hashedPassword = "password";
            string passwordSalt = "salt";
            NewUserModel userModel = new()
            {
                UserFullname = "Abc",
                UserEmail = "abc@xyz.com",
                UserRoles = 1
            };
            SqlParamsModel queryParams = GetParams_CreateAsync(userModel, passwordSalt, hashedPassword);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(userModel, passwordSalt, hashedPassword);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static IEnumerable<UserListItemModel> GetSampleData_RetrieveAllAsync()
        {
            var result = new List<UserListItemModel>
            {
                new UserListItemModel { UserId = 1, UserNameAndEmail = "ABC (abc@xyz.com)" },
                new UserListItemModel { UserId = 2, UserNameAndEmail = "DEF (def@xyz.com)" }
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_get_all_users",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAllAsync_Valid()
        {
            // Arrange
            SqlParamsModel queryParams = GetParams_RetrieveAllAsync();
            IEnumerable<UserListItemModel> expected = GetSampleData_RetrieveAllAsync();
            _mockDataAccess.QueryAsync<UserListItemModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<UserListItemModel> actual = await _sut.RetrieveAllAsync();

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<UserListItemModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).UserId, actual.ElementAt(i).UserId);
                Assert.Equal(expected.ElementAt(i).UserNameAndEmail, actual.ElementAt(i).UserNameAndEmail);
            }
        }

        private static SqlParamsModel GetParams_RetrieveAllWithRolesAsync(int roles)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_get_users_with_roles",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_roles", roles, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAllWithRolesAsync_Valid()
        {
            // Arrange
            int roles = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAllWithRolesAsync(roles);
            IEnumerable<UserListItemModel> expected = GetSampleData_RetrieveAllAsync();
            _mockDataAccess.QueryAsync<UserListItemModel>(default).ReturnsForAnyArgs(expected);

            // Act
            IEnumerable<UserListItemModel> actual = await _sut.RetrieveAllWithRolesAsync(roles);

            // Assert
            await _mockDataAccess.Received(1).QueryAsync<UserListItemModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.Count(), actual.Count());
            for (int i = 0; i < expected.Count(); i++)
            {
                Assert.Equal(expected.ElementAt(i).UserId, actual.ElementAt(i).UserId);
                Assert.Equal(expected.ElementAt(i).UserNameAndEmail, actual.ElementAt(i).UserNameAndEmail);
            }
        }

        private static UserDetailsModel GetSampleData_RetrieveAsync()
        {
            var result = new UserDetailsModel
            {
                UserId = 1,
                UserFullname = "Abc",
                UserEmail = "abc@xyz.com",
                UserRoles = 1
            };
            return result;
        }
        private static SqlParamsModel GetParams_RetrieveAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_get_user_details",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int userId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(userId);
            UserDetailsModel expected = GetSampleData_RetrieveAsync();
            _mockDataAccess.QueryFirstOrDefaultAsync<UserDetailsModel>(default).ReturnsForAnyArgs(expected);

            // Act
            UserDetailsModel actual = await _sut.RetrieveAsync(userId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<UserDetailsModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.UserId, actual.UserId);
            Assert.Equal(expected.UserFullname, actual.UserFullname);
            Assert.Equal(expected.UserEmail, actual.UserEmail);
            Assert.Equal(expected.UserRoles, actual.UserRoles);
        }

        private static SqlParamsModel GetParams_UpdateAsync(UserDetailsModel userModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_update_user",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userModel.UserId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_email", userModel.UserEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_fullname", userModel.UserFullname, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_roles", userModel.UserRoles, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            UserDetailsModel userModel = GetSampleData_RetrieveAsync();
            SqlParamsModel queryParams = GetParams_UpdateAsync(userModel);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(userModel);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_ChangePasswordAsync(int userId, string userPassword)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_change_password",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_password", userPassword, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task ChangePasswordAsync_Valid()
        {
            // Arrange
            int userId = 1;
            string userPassword = "password";
            SqlParamsModel queryParams = GetParams_ChangePasswordAsync(userId, userPassword);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.ChangePasswordAsync(userId, userPassword);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_delete_user",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int userId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(userId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(userId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
