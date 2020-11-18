using Dapper.Oracle;
using System;
using System.Linq;

namespace CCMS.Tests.DbServices
{
    public class EquatableOracleDynamicParameters
    {
        public static bool Equal(OracleDynamicParameters param1, OracleDynamicParameters param2)
        {
            if (param1 is null || param2 is null || param1.ArrayBindCount != param2.ArrayBindCount)
            {
                return false;
            }
            bool result = false;
            foreach (var item in param1.ParameterNames)
            {
                result = param2.ParameterNames.Contains(item);
            }
            return result;
        }
    }
}
