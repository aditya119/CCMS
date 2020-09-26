﻿using System.Collections.Generic;
using CCMS.Shared.Models;
using System.Threading.Tasks;
using Dapper.Oracle;
using System.Data;
using CCMS.Server.DbDataAccess;

namespace CCMS.Server.DbServices
{
    public class ActorTypesService : IActorTypesService
    {
        private readonly IOracleDataAccess _dataAccess;

        public ActorTypesService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<IEnumerable<ActorTypeModel>> RetrieveAllAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<ActorTypeModel>("pkg_actor_types.p_get_all_actor_types", parameters);
        }
    }
}