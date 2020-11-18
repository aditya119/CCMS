using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbDataAccess
{
    public interface IOracleDataAccess
    {
        Task<int> ExecuteAsync(SqlParamsModel sqlModel);
        Task<int> ExecuteAsync(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteTransactionAsync(IEnumerable<SqlParamsModel> sqlModels);
        Task<IEnumerable<T>> QueryAsync<T>(SqlParamsModel sqlModel);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        public Task<T> QueryFirstOrDefaultAsync<T>(SqlParamsModel sqlModel);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
    }
}