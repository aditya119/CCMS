using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.AttachmentModels;
using Dapper.Oracle;
using NSubstitute;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.DbServices
{
    public class AttachmentsServiceTests
    {
        private readonly AttachmentsService _sut;
        private readonly IOracleDataAccess _mockDataAccess = Substitute.For<IOracleDataAccess>();
        public AttachmentsServiceTests()
        {
            _sut = new AttachmentsService(_mockDataAccess);
        }

        private static SqlParamsModel GetParams_CreateAsync(NewAttachmentModel attachmentModel, byte[] attachmentFile)
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
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            NewAttachmentModel attachmentModel = new()
            {
                Filename = "abc.pdf",
                LastUpdateBy = 1
            };
            byte[] attachmentFile = Encoding.UTF8.GetBytes("sampledata");
            SqlParamsModel queryParams = GetParams_CreateAsync(attachmentModel, attachmentFile);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(attachmentModel, attachmentFile);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_RetrieveAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_get_attachment_details",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        private static AttachmentItemModel GetSampleData()
        {
            AttachmentItemModel result = new() { AttachmentId = 1, Filename = "abc.pdf", LastUpdateBy = 1 };
            return result;
        }
        [Fact]
        public async Task RetrieveAsync_Valid()
        {
            // Arrange
            int attachmentId = 1;
            SqlParamsModel queryParams = GetParams_RetrieveAsync(attachmentId);
            AttachmentItemModel expected = GetSampleData();
            _mockDataAccess.QueryFirstOrDefaultAsync<AttachmentItemModel>(default).ReturnsForAnyArgs(expected);

            // Act
            AttachmentItemModel actual = await _sut.RetrieveAsync(attachmentId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<AttachmentItemModel>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.True(actual is not null);
            Assert.Equal(expected.AttachmentId, actual.AttachmentId);
            Assert.Equal(expected.Filename, actual.Filename);
            Assert.Equal(expected.LastUpdateBy, actual.LastUpdateBy);
        }

        private static SqlParamsModel GetParams_DownloadAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_download_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_attachment_file", dbType: OracleMappingType.Blob, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task DownloadAsync_Valid()
        {
            // Arrange
            int attachmentId = 1;
            SqlParamsModel queryParams = GetParams_DownloadAsync(attachmentId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(0);

            // Act
            await _sut.DownloadAsync(attachmentId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_UpdateAsync(AttachmentItemModel attachmentModel, byte[] attachmentFile)
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
            return sqlModel;
        }
        [Fact]
        public async Task UpdateAsync_Valid()
        {
            // Arrange
            AttachmentItemModel attachmentModel = new()
            {
                AttachmentId = 1,
                Filename = "abcd.pdf",
                LastUpdateBy = 1
            };
            byte[] attachmentFile = Encoding.UTF8.GetBytes("sampledata");
            SqlParamsModel queryParams = GetParams_UpdateAsync(attachmentModel, attachmentFile);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.UpdateAsync(attachmentModel, attachmentFile);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }

        private static SqlParamsModel GetParams_DeleteAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_delete_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int attachmentId = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(attachmentId);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(attachmentId);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
