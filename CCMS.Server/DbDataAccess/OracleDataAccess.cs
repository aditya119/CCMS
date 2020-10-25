using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace CCMS.Server.DbDataAccess
{
    public class OracleDataAccess : IOracleDataAccess
    {
        private readonly IDbConnection _dbConnection;

        public OracleDataAccess(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(ExecuteSqlModel sqlModel)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryFirstOrDefaultAsync<T>(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, commandType: commandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(ExecuteSqlModel sqlModel)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryAsync<T>(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            return _dbConnection.QueryAsync<T>(sql, parameters, commandType: commandType);
        }

        public Task<int> ExecuteAsync(ExecuteSqlModel sqlModel)
        {
            return _dbConnection.ExecuteAsync(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
        }

        public Task<int> ExecuteAsync(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            return _dbConnection.ExecuteAsync(sql, parameters, commandType: commandType);
        }

        public async Task<int> ExecuteTransactionAsync(IEnumerable<ExecuteSqlModel> sqlModels)
        {
            int sumRowsImpacted = 0;
            using (new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var sqlModel in sqlModels)
                {
                    sumRowsImpacted += await _dbConnection.ExecuteAsync(sqlModel.Sql, sqlModel.Parameters, commandType: sqlModel.CommandType);
                }
            }
            return sumRowsImpacted;
        }
    }
}
