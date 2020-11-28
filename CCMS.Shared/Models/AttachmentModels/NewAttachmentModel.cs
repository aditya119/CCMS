using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.AttachmentModels
{
    public class NewAttachmentModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Filename too long")]
        public string Filename { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int LastUpdateBy { get; set; }
    }
}
