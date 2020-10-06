using System.IO;
using System.Threading.Tasks;
using CCMS.Server.DbServices;
using CCMS.Server.Utilities;
using CCMS.Shared.Models.AttachmentModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CCMS.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticateSession]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentsService _attachmentsService;

        public AttachmentController(IAttachmentsService attachmentsService)
        {
            _attachmentsService = attachmentsService;
        }

        [HttpGet]
        [Route("{attachmentId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<AttachmentItemModel>> GetAttachmentDetails(int attachmentId)
        {
            if (attachmentId < 1)
            {
                return UnprocessableEntity("Invalid AttachmentId");
            }
            AttachmentItemModel attachmentItem = await _attachmentsService.RetrieveAsync(attachmentId);
            if (attachmentItem == null)
            {
                return NotFound($"AttachmentId {attachmentId}, not found.");
            }
            return Ok(attachmentItem);
        }

        [HttpGet]
        [Route("{attachmentId:int}/download")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [Authorize]
        public async Task<ActionResult<byte[]>> DownloadAttachment(int attachmentId)
        {
            if (attachmentId < 1)
            {
                return UnprocessableEntity("Invalid AttachmentId");
            }
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", attachmentId.ToString());
            if (System.IO.File.Exists(filePath) == false)
            {
                return NotFound($"AttachmentId {attachmentId}, not found.");
            }
            byte[] attachmentFile = await System.IO.File.ReadAllBytesAsync(filePath);
            return Ok(attachmentFile);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Post([FromForm] IFormFile uploadedAttachment)
        {
            var attachment = new NewAttachmentModel
            {
                Filename = uploadedAttachment.FileName
            };
            if (TryValidateModel(attachment) == false)
            {
                return ValidationProblem();
            }
            int attachmentId = await _attachmentsService.CreateAsync(attachment);
            using (var ms = new MemoryStream())
            {
                uploadedAttachment.CopyTo(ms);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", attachmentId.ToString());
                await System.IO.File.WriteAllBytesAsync(filePath, ms.ToArray());
            }
            return Created("/api/attachment/", attachmentId);
        }

        [HttpPut]
        [Route("{attachmentId:int}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<IActionResult> Put([FromForm] IFormFile uploadedAttachment, int attachmentId)
        {
            var attachment = new AttachmentItemModel
            {
                AttachmentId = attachmentId,
                Filename = uploadedAttachment.FileName
            };
            if (TryValidateModel(attachment) == false)
            {
                return ValidationProblem();
            }
            await _attachmentsService.UpdateAsync(attachment);
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", attachmentId.ToString());
            System.IO.File.Delete(filePath);
            using (var ms = new MemoryStream())
            {
                uploadedAttachment.CopyTo(ms);
                await System.IO.File.WriteAllBytesAsync(filePath, ms.ToArray());
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("{attachmentId:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Delete(int attachmentId)
        {
            if (attachmentId < 1)
            {
                return UnprocessableEntity("Invalid AttachmentId");
            }
            await _attachmentsService.DeleteAsync(attachmentId);
            return NoContent();
        }
    }
}
