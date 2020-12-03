using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.LocationModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class LocationsService : ILocationsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public LocationsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewLocationModel locationModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_create_new_location",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_name", locationModel.LocationName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_location_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            int locationId = (int)sqlModel.Parameters.Get<decimal>("po_location_id");
            return locationId;
        }

        public async Task<IEnumerable<LocationDetailsModel>> RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_get_all_locations",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<LocationDetailsModel>(sqlModel);
        }

        public async Task<LocationDetailsModel> RetrieveAsync(int locationId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_get_location_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_id", locationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<LocationDetailsModel>(sqlModel);
        }

        public async Task UpdateAsync(LocationDetailsModel locationModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_update_location",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_id", locationModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_location_name", locationModel.LocationName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int locationId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_locations.p_delete_location",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_location_id", locationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
