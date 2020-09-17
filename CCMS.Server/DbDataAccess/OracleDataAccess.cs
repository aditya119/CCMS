using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbDataAccess
{
    public class OracleDataAccess : IOracleDataAccess
    {
        private readonly IDbConnection _dbConnection;

        public OracleDataAccess(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryAsync<T>(sql, parameters, commandType: commandType);
        }

        public Task<int> ExecuteAsync(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            return _dbConnection.ExecuteAsync(sql, parameters, commandType: commandType);
        }
    }
}
