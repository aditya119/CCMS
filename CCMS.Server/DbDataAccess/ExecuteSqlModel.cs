using Dapper.Oracle;
using System.Data;

namespace CCMS.Server.DbDataAccess
{
    public class ExecuteSqlModel
    {
        public string Sql { get; set; }
        public OracleDynamicParameters Parameters { get; set; } = null;
        public CommandType CommandType { get; set; } = CommandType.StoredProcedure;
    }
}
