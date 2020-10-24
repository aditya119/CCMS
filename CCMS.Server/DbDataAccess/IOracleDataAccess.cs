using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbDataAccess
{
    public interface IOracleDataAccess
    {
        Task<int> ExecuteAsync(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<T> QueryFirstOrDefaultAsync<T>(string sql, object parameters = null, CommandType commandType = CommandType.StoredProcedure);
        Task<int> ExecuteTransactionAsync(IEnumerable<ExecuteSqlModel> sqlModels);
    }
}