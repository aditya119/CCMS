using Dapper.Oracle;
using System.Data;

namespace CCMS.Server.Services.DbDataAccessService
{
    public class SqlParamsModel
    {
        public string Sql { get; set; }
        public OracleDynamicParameters Parameters { get; set; } = null;
        public CommandType CommandType { get; set; } = CommandType.StoredProcedure;
    }
}
