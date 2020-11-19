using Dapper.Oracle;
using System;
using System.Linq;
using static Dapper.Oracle.OracleDynamicParameters;

namespace CCMS.Tests.DbServices
{
    public class EquatableOracleDynamicParameters
    {
        public static bool AreEqual(OracleDynamicParameters param1, OracleDynamicParameters param2)
        {
            if (param1 is null || param2 is null || param1.ArrayBindCount != param2.ArrayBindCount)
            {
                return false;
            }
            bool result = false;
            foreach (var item in param1.ParameterNames)
            {
                result = param2.ParameterNames.Contains(item);
                if (result == false)
                {
                    break;
                }

                OracleParameterInfo param1Info = param1.GetParameter(item);
                OracleParameterInfo param2Info = param2.GetParameter(item);

                result = param1Info.DbType == param2Info.DbType;
                if (result == false)
                {
                    break;
                }

                result = param1Info.ParameterDirection == param2Info.ParameterDirection;
                if (result == false)
                {
                    break;
                }

                result = param1Info.Size == param2Info.Size;
                if (result == false)
                {
                    break;
                }
            }
            return result;
        }
    }
}
