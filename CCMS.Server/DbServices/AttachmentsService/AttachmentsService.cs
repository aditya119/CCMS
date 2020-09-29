using CCMS.Server.DbDataAccess;
using CCMS.Shared.Models.AttachmentModels;
using Dapper.Oracle;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public class AttachmentsService : IAttachmentsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public AttachmentsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewAttachmentModel attachmentModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_filename", attachmentModel.Filename, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            parameters.Add("po_attachment_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync("pkg_attachments.p_create_new_attachment", parameters);

            int attachmentId = (int)parameters.Get<decimal>("po_attachment_id");
            return attachmentId;
        }

        public async Task<AttachmentItemModel> RetrieveAsync(int attachmentId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<AttachmentItemModel>("pkg_attachments.p_get_attachment_details", parameters);
        }

        public async Task UpdateAsync(AttachmentItemModel attachmentModel)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_attachment_id", attachmentModel.AttachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            parameters.Add("pi_filename", attachmentModel.Filename, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_attachments.p_update_attachment", parameters);
        }

        public async Task DeleteAsync(int attachmentId)
        {
            var parameters = new OracleDynamicParameters();
            parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync("pkg_attachments.p_delete_attachment", parameters);
        }
    }
}
