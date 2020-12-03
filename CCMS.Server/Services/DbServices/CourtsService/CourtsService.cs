using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.CourtModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
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
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_create_new_court",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_name", courtModel.CourtName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_court_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            int courtId = (int)sqlModel.Parameters.Get<decimal>("po_court_id");
            return courtId;
        }

        public async Task<IEnumerable<CourtDetailsModel>> RetrieveAllAsync()
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_get_all_courts",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<CourtDetailsModel>(sqlModel);
        }

        public async Task<CourtDetailsModel> RetrieveAsync(int courtId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_get_court_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_id", courtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<CourtDetailsModel>(sqlModel);
        }

        public async Task UpdateAsync(CourtDetailsModel courtModel)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_update_court",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_id", courtModel.CourtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_court_name", courtModel.CourtName, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int courtId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_courts.p_delete_court",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_court_id", courtId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
