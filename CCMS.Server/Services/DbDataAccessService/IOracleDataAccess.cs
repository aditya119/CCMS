using System.Collections.Generic;
using System.Threading.Tasks;
using CCMS.Server.Models;

namespace CCMS.Server.Services.DbDataAccessService
{
    public interface IOracleDataAccess
    {
        Task<int> ExecuteAsync(SqlParamsModel sqlModel);
        Task<IEnumerable<T>> QueryAsync<T>(SqlParamsModel sqlModel);
        public Task<T> QueryFirstOrDefaultAsync<T>(SqlParamsModel sqlModel);
    }
}