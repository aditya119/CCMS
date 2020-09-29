using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.AttachmentModels
{
    public class AttachmentItemModel : NewAttachmentModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int AttachmentId { get; set; }
    }
}
