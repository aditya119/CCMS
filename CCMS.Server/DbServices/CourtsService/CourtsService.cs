using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models.CourtModels;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class CourtsService : ICourtsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public CourtsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewCourtModel courtModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_court_name", courtModel.CourtName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_court_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_courts.p_create_new_court", parameters);

            int courtId = (int)parameters.Get<decimal>("po_court_id");
            return courtId;
        }

        public async Task<IEnumerable<CourtDetailsModel>> RetrieveAllAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CourtDetailsModel>("pkg_courts.p_get_all_courts", parameters);
        }

        public async Task<CourtDetailsModel> RetrieveAsync(int courtId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_court_id", courtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CourtDetailsModel>("pkg_courts.p_get_court_details", parameters);
        }

        public async Task UpdateAsync(CourtDetailsModel courtModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_court_id", courtModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_court_name", courtModel.CourtName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_courts.p_update_court", parameters);
        }

        public async Task DeleteAsync(int courtId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_court_id", courtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_courts.p_delete_court", parameters);
        }
    }
}
