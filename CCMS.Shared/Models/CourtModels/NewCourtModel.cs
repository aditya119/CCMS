using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CourtModels
{
    public class NewCourtModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Court name too long")]
        public string CourtName { get; set; }
    }
}
