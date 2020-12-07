using CCMS.Server.Services.DbDataAccessService;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.AttachmentModels;
using CCMS.Server.Models;
using Dapper.Oracle;
using NSubstitute;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;

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

        private static SqlParamsModel GetParams_CreateAsync(NewAttachmentModel attachmentModel, byte[] attachmentFile, int currUser)
        {
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
            return sqlModel;
        }
        [Fact]
        public async Task CreateAsync_Valid()
        {
            // Arrange
            NewAttachmentModel attachmentModel = new(new List<string> { ".pdf" })
            {
                Filename = "abc.pdf",
                ContentType = "application/pdf"
            };
            int currUser = 1;
            byte[] attachmentFile = Encoding.UTF8.GetBytes("sampledata");
            SqlParamsModel queryParams = GetParams_CreateAsync(attachmentModel, attachmentFile, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.CreateAsync(attachmentModel, attachmentFile, currUser);

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
            AttachmentItemModel result = new() { AttachmentId = 1, Filename = "abc.pdf", ContentType = "application/pdf" };
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
            Assert.Equal(expected.ContentType, actual.ContentType);
        }

        private static SqlParamsModel GetParams_DownloadAsync(int attachmentId)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_download_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("po_cursor", dbType: OracleMappingType.RefCursor, direction: ParameterDirection.Output);
            return sqlModel;
        }
        [Fact]
        public async Task DownloadAsync_Valid()
        {
            // Arrange
            int attachmentId = 1;
            SqlParamsModel queryParams = GetParams_DownloadAsync(attachmentId);
            byte[] expected = Encoding.UTF8.GetBytes("sampledata");
            _mockDataAccess.QueryFirstOrDefaultAsync<byte[]>(default).ReturnsForAnyArgs(expected);

            // Act
            byte[]actual = await _sut.DownloadAsync(attachmentId);

            // Assert
            await _mockDataAccess.Received(1).QueryFirstOrDefaultAsync<byte[]>(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
            Assert.Equal(expected, actual);
        }

        private static SqlParamsModel GetParams_DeleteAsync(int attachmentId, int currUser)
        {
            var sqlModel = new SqlParamsModel
            {
                Sql = "pkg_attachments.p_delete_attachment",
                Parameters = new OracleDynamicParameters()
            };
            sqlModel.Parameters.Add("pi_attachment_id", attachmentId, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            sqlModel.Parameters.Add("pi_update_by", currUser, dbType: OracleMappingType.Int32, ParameterDirection.Input);
            return sqlModel;
        }
        [Fact]
        public async Task DeleteAsync_Valid()
        {
            // Arrange
            int attachmentId = 1;
            int currUser = 1;
            SqlParamsModel queryParams = GetParams_DeleteAsync(attachmentId, currUser);
            _mockDataAccess.ExecuteAsync(default).ReturnsForAnyArgs(1);

            // Act
            await _sut.DeleteAsync(attachmentId, currUser);

            // Assert
            await _mockDataAccess.Received(1).ExecuteAsync(Arg.Is<SqlParamsModel>(
                p => p.Sql == queryParams.Sql
                && p.CommandType == queryParams.CommandType
                && EquatableOracleDynamicParameters.AreEqual(p.Parameters, queryParams.Parameters)
                ));
        }
    }
}
