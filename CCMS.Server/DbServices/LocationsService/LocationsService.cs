using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models.LocationModels;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
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
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_location_name", locationModel.LocationName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_location_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_locations.p_create_new_location", parameters);

            int locationId = (int)parameters.Get<decimal>("po_location_id");
            return locationId;
        }

        public async Task<IEnumerable<LocationDetailsModel>> RetrieveAllAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<LocationDetailsModel>("pkg_locations.p_get_all_locations", parameters);
        }

        public async Task<LocationDetailsModel> RetrieveAsync(int locationId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_location_id", locationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<LocationDetailsModel>("pkg_locations.p_get_location_details", parameters);
        }

        public async Task UpdateAsync(LocationDetailsModel locationModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_location_id", locationModel.LocationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_location_name", locationModel.LocationName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_locations.p_update_location", parameters);
        }

        public async Task DeleteAsync(int locationId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_location_id", locationId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_locations.p_delete_location", parameters);
        }
    }
}
