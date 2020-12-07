using System.IO;
using System.Threading.Tasks;
using CCMS.Server.Services.DbServices;
using CCMS.Server.Services;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.AttachmentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [AuthenticateSession]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentsService _attachmentsService;
        private readonly ISessionService _sessionService;
        private readonly List<string> _allowedExtensions;
        public AttachmentController(IAttachmentsService attachmentsService, ISessionService sessionService,
            IConfiguration config)
        {
            _attachmentsService = attachmentsService;
            _sessionService = sessionService;
            _allowedExtensions = config.GetSection("FileUpload:AllowedExtensions").Get<List<string>>();
        }

        [HttpGet]
        [Route("{attachmentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AttachmentItemModel>> GetAttachmentDetails(int attachmentId)
        {
            if (attachmentId < 0)
            {
                return UnprocessableEntity($"Invalid AttachmentId: {attachmentId}");
            }
            AttachmentItemModel attachmentItem = await _attachmentsService.RetrieveAsync(attachmentId);
            if (attachmentItem is null)
            {
                return NotFound($"AttachmentId {attachmentId}, not found.");
            }
            return Ok(attachmentItem);
        }

        [HttpGet]
        [Route("{attachmentId:int}/download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            if (attachmentId < 1)
            {
                return UnprocessableEntity($"Invalid AttachmentId: {attachmentId}");
            }
            AttachmentItemModel attachmentItem = await _attachmentsService.RetrieveAsync(attachmentId);
            if (attachmentItem is null)
            {
                return NotFound($"AttachmentId {attachmentId}, not found.");
            }
            byte[] attachmentFile = await _attachmentsService.DownloadAsync(attachmentId);
            return File(attachmentFile, attachmentItem.ContentType, attachmentItem.Filename);
        }

        [HttpGet]
        [Route("allowed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public ActionResult<IEnumerable<string>> GetAllowedExtensions()
        {
            return Ok(_allowedExtensions);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Post(IFormFile file)
        {
            if (file is null)
            {
                return ValidationProblem("A non-empty request body is required.");
            }
            var attachment = new NewAttachmentModel(_allowedExtensions)
            {
                Filename = file.FileName,
                ContentType = file.ContentType
            };
            if (TryValidateModel(attachment) == false)
            {
                return ValidationProblem();
            }
            byte[] attachmentFile;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                attachmentFile = ms.ToArray();
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            int attachmentId = await _attachmentsService.CreateAsync(attachment, attachmentFile, currUser);
            return Created("/api/attachment/", attachmentId);
        }

        [HttpDelete]
        [Route("{attachmentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Delete(int attachmentId)
        {
            if (attachmentId < 1)
            {
                return UnprocessableEntity($"Invalid AttachmentId: {attachmentId}");
            }
            int currUser = _sessionService.GetUserId(HttpContext);
            await _attachmentsService.DeleteAsync(attachmentId, currUser);
            return NoContent();
        }
    }
}
