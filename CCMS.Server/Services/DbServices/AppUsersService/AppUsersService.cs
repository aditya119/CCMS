﻿using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.AppUserModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class AppUsersService : IAppUsersService
    {
        private readonly IOracleDataAccess _dataAccess;

        public AppUsersService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewUserModel userModel, string passwordSalt, string hashedPassword)
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

            await _dataAccess.ExecuteAsync(sqlModel);

            int userId = (int)sqlModel.Parameters.Get<decimal>("po_user_id");
            return userId;
        }

        public async Task<IEnumerable<UserListItemModel>> RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_get_all_users",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<UserListItemModel>(sqlModel);
        }

        public async Task<IEnumerable<UserListItemModel>> RetrieveAllWithRolesAsync(int roles)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_get_users_with_roles",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_roles", roles, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<UserListItemModel>(sqlModel);
        }

        public async Task<UserDetailsModel> RetrieveAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_get_user_details",
                Parameters = new OracleDynamicParameters()
            };

            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            
            return await _dataAccess.QueryFirstOrDefaultAsync<UserDetailsModel>(sqlModel);
        }

        public async Task UpdateAsync(UserDetailsModel userModel)
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

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task ChangePasswordAsync(int userId, string userPassword)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_change_password",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_user_password", userPassword, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task UnlockAccountAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_unlock_account",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int userId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_app_users.p_delete_user",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_user_id", userId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
