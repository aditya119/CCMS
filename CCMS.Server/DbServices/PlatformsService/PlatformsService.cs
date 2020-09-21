﻿using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class PlatformsService : IPlatformsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public PlatformsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<PlatformModel>> GetAllPlatforms()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<PlatformModel>("pkg_platforms.p_get_all_platforms", parameters);
        }
    }
}
