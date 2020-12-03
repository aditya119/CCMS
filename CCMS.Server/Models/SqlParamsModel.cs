using Dapper.Oracle;
using System.Data;

namespace CCMS.Server.Models
{
    public class SqlParamsModel
    {
        public string Sql { get; init; }
        public OracleDynamicParameters Parameters { get; init; } = null;
        public CommandType CommandType { get; init; } = CommandType.StoredProcedure;
    }
}
