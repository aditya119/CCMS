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

        public async Task<int> CreateAsync(NewAttachmentModel attachmentModel, byte[] attachmentFile, int currUser)
        { // using proc to insert blob gives error: ORA-22835: Buffer too small for CLOB to CHAR or BLOB to RAW conversion
            var paramObj = new
            {
                attachmentModel.Filename,
                attachmentModel.ContentType,
                AttachmentFile = attachmentFile,
                CurrUser = currUser
            };
            var sqlModel = new SqlParamsModel
            {
                Sql = "insert into attachments ("
                            + "filename,"
                            + "attachment_file,"
                            + "content_type,"
                            + "last_update_by"
                        + ") values ("
                            + ":Filename,"
                            + ":AttachmentFile,"
                            + ":ContentType,"
                            + ":CurrUser"
                        + ") returning "
                            + "attachment_id"
                        + " into "
                            + ":attachment_id",
                Parameters = new OracleDynamicParameters(paramObj),
                CommandType = CommandType.Text
            };
            sqlModel.Parameters.Add(name: "attachment_id", dbType: OracleMappingType.Int32, direction: ParameterDirection.Output);
            await _dataAccess.ExecuteAsync(sqlModel);
            return sqlModel.Parameters.Get<int>("attachment_id");
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
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);

            return await _dataAccess.QueryFirstOrDefaultAsync<byte[]>(sqlModel);
        }

        public async Task DeleteAsync(int attachmentId, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_delete_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);

            await _dataAccess.ExecuteAsync(sqlModel);
        }
    }
}
