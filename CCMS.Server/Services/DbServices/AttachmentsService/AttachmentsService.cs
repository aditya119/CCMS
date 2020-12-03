using CCMS.Server.Services.DbDataAccessService;
using CCMS.Shared.Models.AttachmentModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using System.Data;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public class AttachmentsService : IAttachmentsService
    {
        private readonly IOracleDataAccess _dataAccess;

        public AttachmentsService(IOracleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<int> CreateAsync(NewAttachmentModel attachmentModel, byte[] attachmentFile)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_create_new_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_filename", attachmentModel.Filename, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_attachment_file", attachmentFile, dbType: OracleMappingType.Blob, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_create_by", attachmentModel.LastUpdateBy, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_attachment_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);

            int attachmentId = (int)sqlModel.Parameters.Get<decimal>("po_attachment_id");
            return attachmentId;
        }

        public async Task<AttachmentItemModel> RetrieveAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_get_attachment_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<AttachmentItemModel>(sqlModel);
        }

        public async Task<byte[]> DownloadAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_download_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_attachment_file", dbType: OracleMappingType.Blob, direction: ParameterDirection.Output);

            await _dataAccess.ExecuteAsync(sqlModel);
            return sqlModel.Parameters.Get<byte[]>("po_attachment_file");
        }

        public async Task UpdateAsync(AttachmentItemModel attachmentModel, byte[] attachmentFile)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_update_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentModel.AttachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_filename", attachmentModel.Filename, dbType: OracleMappingType.Varchar2, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_attachment_file", attachmentFile, dbType: OracleMappingType.Blob, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", attachmentModel.LastUpdateBy, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }

        public async Task DeleteAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_delete_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
