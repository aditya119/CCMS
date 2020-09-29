using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.AttachmentModels
{
    public class NewAttachmentModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Filename too long")]
        public string Filename { get; set; }
    }
}
