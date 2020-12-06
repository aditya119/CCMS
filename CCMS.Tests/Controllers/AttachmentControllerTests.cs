using CCMS.Server.Controllers;
using CCMS.Server.Services;
using CCMS.Server.Services.DbServices;
using CCMS.Shared.Models.AttachmentModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CCMS.Tests.Controllers
{
    public class AttachmentControllerTests
    {
        private readonly AttachmentController _sut;
        private readonly IAttachmentsService _mockAttachmentsService = Substitute.For<IAttachmentsService>();
        private readonly ISessionService _mockSessionService = Substitute.For<ISessionService>();
        public AttachmentControllerTests()
        {
            _sut = new AttachmentController(_mockAttachmentsService, _mockSessionService);
        }

        public static AttachmentItemModel GetSampleData_AttachmentItem(int attachmentId)
        {
            var result = new AttachmentItemModel
            {
                AttachmentId = attachmentId,
                Filename = "filename.pdf"
            };
            return result;
        }

        [Fact]
        public async Task GetAttachmentDetails_UnprocessableEntity()
        {
            // Arrange
            int attachmentId = 0;
            string expectedError = $"Invalid AttachmentId: {attachmentId}";

            // Act
            ActionResult<AttachmentItemModel> response = await _sut.GetAttachmentDetails(attachmentId);

            // Assert
            await _mockAttachmentsService.DidNotReceiveWithAnyArgs().RetrieveAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetAttachmentDetails_NotFound()
        {
            // Arrange
            int attachmentId = 1;
            AttachmentItemModel expected = null;
            _mockAttachmentsService.RetrieveAsync(attachmentId).Returns(expected);
            string expectedError = $"AttachmentId {attachmentId}, not found.";

            // Act
            ActionResult<AttachmentItemModel> response = await _sut.GetAttachmentDetails(attachmentId);

            // Assert
            await _mockAttachmentsService.Received(1).RetrieveAsync(attachmentId);
            var createdAtActionResult = Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task GetAttachmentDetails_Valid()
        {
            // Arrange
            int attachmentId = 1;
            AttachmentItemModel expected = GetSampleData_AttachmentItem(attachmentId);
            _mockAttachmentsService.RetrieveAsync(attachmentId).Returns(expected);

            // Act
            ActionResult<AttachmentItemModel> response = await _sut.GetAttachmentDetails(attachmentId);

            // Assert
            await _mockAttachmentsService.Received(1).RetrieveAsync(attachmentId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            AttachmentItemModel actual = (AttachmentItemModel)createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected.AttachmentId, actual.AttachmentId);
            Assert.Equal(expected.Filename, actual.Filename);
        }

        [Fact]
        public async Task DownloadAttachment_UnprocessableEntity()
        {
            // Arrange
            int attachmentId = 0;
            string expectedError = $"Invalid AttachmentId: {attachmentId}";

            // Act
            ActionResult<byte[]> response = await _sut.DownloadAttachment(attachmentId);

            // Assert
            await _mockAttachmentsService.DidNotReceiveWithAnyArgs().DownloadAsync(default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task DownloadAttachment_NotFound()
        {
            // Arrange
            int attachmentId = 1;
            byte[] expected = null;
            _mockAttachmentsService.DownloadAsync(attachmentId).Returns(expected);
            string expectedError = $"AttachmentId {attachmentId}, not found.";

            // Act
            ActionResult<byte[]> response = await _sut.DownloadAttachment(attachmentId);

            // Assert
            await _mockAttachmentsService.Received(1).DownloadAsync(attachmentId);
            var createdAtActionResult = Assert.IsType<NotFoundObjectResult>(response.Result);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task DownloadAttachment_Valid()
        {
            // Arrange
            int attachmentId = 1;
            byte[] expected = Encoding.UTF8.GetBytes("sampledata");
            _mockAttachmentsService.DownloadAsync(attachmentId).Returns(expected);

            // Act
            ActionResult<byte[]> response = await _sut.DownloadAttachment(attachmentId);

            // Assert
            await _mockAttachmentsService.Received(1).DownloadAsync(attachmentId);
            var createdAtActionResult = Assert.IsType<OkObjectResult>(response.Result);
            byte[] actual = (byte[])createdAtActionResult.Value;
            Assert.True(actual is not null);
            Assert.Equal(expected, actual);
        }
        /*
        [Fact]
        public async Task Post_ValidationProblem()
        {
            // Arrange
            IFormFile mockFile = Substitute.For<IFormFile>();
            mockFile.FileName.Returns("abc");
            //_sut.TryValidateModel(default).ReturnsForAnyArgs(false);

            // Act
            await _sut.Post(mockFile);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockAttachmentsService.DidNotReceiveWithAnyArgs().CreateAsync(default, default, default);
            Assert.False(_sut.ModelState.IsValid);
        }*/

        [Fact]
        public async Task Delete_UnprocessableEntity()
        {
            // Arrange
            int attachmentId = 0;
            string expectedError = $"Invalid AttachmentId: {attachmentId}";

            // Act
            IActionResult response = await _sut.Delete(attachmentId);

            // Assert
            _mockSessionService.DidNotReceiveWithAnyArgs().GetUserId(default);
            await _mockAttachmentsService.DidNotReceiveWithAnyArgs().DeleteAsync(default, default);
            var createdAtActionResult = Assert.IsType<UnprocessableEntityObjectResult>(response);
            Assert.Equal(expectedError, createdAtActionResult.Value);
        }

        [Fact]
        public async Task Delete_Valid()
        {
            // Arrange
            int attachmentId = 1;
            int currUser = 1;
            int deleteAsyncCalled = 0;
            _mockSessionService.GetUserId(default).ReturnsForAnyArgs(currUser);
            _mockAttachmentsService.When(x => x.DeleteAsync(attachmentId, currUser))
                .Do(x => deleteAsyncCalled++);

            // Act
            IActionResult response = await _sut.Delete(attachmentId);

            // Assert
            _mockSessionService.ReceivedWithAnyArgs(1).GetUserId(default);
            await _mockAttachmentsService.Received(1).DeleteAsync(attachmentId, currUser);
            Assert.IsType<NoContentResult>(response);
        }
    }
}
