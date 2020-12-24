using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using CCMS.Server.Models;

namespace CCMS.Server.Services.DbDataAccessService
{
    public class OracleDataAccess : IOracleDataAccess
    {
        private readonly IDbConnection _dbConnection;

        public OracleDataAccess(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(SqlParamsModel sqlModel)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryFirstOrDefaultAsync<T>(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(SqlParamsModel sqlModel)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryAsync<T>(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
        }

        public Task<int> ExecuteAsync(SqlParamsModel sqlModel)
        {
            return _dbConnection.ExecuteAsync(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
        }
    }
}
