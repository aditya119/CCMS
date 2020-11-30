using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbDataAccessService
{
    public interface IOracleDataAccess
    {
        Task<int> ExecuteAsync(SqlParamsModel sqlModel);
        Task<int> ExecuteTransactionAsync(IEnumerable<SqlParamsModel> sqlModels);
        Task<IEnumerable<T>> QueryAsync<T>(SqlParamsModel sqlModel);
        public Task<T> QueryFirstOrDefaultAsync<T>(SqlParamsModel sqlModel);
    }
}