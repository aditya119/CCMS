using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.LawyerModels
{
    public class NewLawyerModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Full name too long")]
        public string LawyerFullname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(1000, ErrorMessage = "E-mail Address too long")]
        public string LawyerEmail { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Phone Number too long")]
        public string LawyerPhone { get; set; }

        [Required]
        [StringLength(4000, ErrorMessage = "Address too long")]
        public string LawyerAddress { get; set; }
    }
}
