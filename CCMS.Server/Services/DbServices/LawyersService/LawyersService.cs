using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.LawyerModels;
using Dapper.Oracle;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class LawyersService : ILawyersService
    {
        private readonly IOracleDataAccess _dataAccess;

        public LawyersService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewLawyerModel lawyerModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_lawyer_email", lawyerModel.LawyerEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_lawyer_fullname", lawyerModel.LawyerFullname, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_lawyer_phone", lawyerModel.LawyerPhone, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_lawyer_address", lawyerModel.LawyerAddress, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_lawyer_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_lawyers.p_create_new_lawyer", parameters);

            int lawyerId = (int)parameters.Get<decimal>("po_lawyer_id");
            return lawyerId;
        }

        public async Task<IEnumerable<LawyerListItemModel>> RetrieveAllAsync()
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryAsync<LawyerListItemModel>("pkg_lawyers.p_get_all_lawyers", parameters);
        }

        public async Task<LawyerDetailsModel> RetrieveAsync(int lawyerId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_lawyer_id", lawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<LawyerDetailsModel>("pkg_lawyers.p_get_lawyer_details", parameters);
        }

        public async Task UpdateAsync(LawyerDetailsModel lawyerModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_lawyer_id", lawyerModel.LawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_lawyer_email", lawyerModel.LawyerEmail, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_lawyer_fullname", lawyerModel.LawyerFullname, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_lawyer_phone", lawyerModel.LawyerPhone, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("pi_lawyer_address", lawyerModel.LawyerAddress, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_lawyers.p_update_lawyer", parameters);
        }

        public async Task DeleteAsync(int lawyerId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_lawyer_id", lawyerId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_lawyers.p_delete_lawyer", parameters);
        }
    }
}
