using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CourtModels
{
    public class CourtDetailsModel : NewCourtModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CourtId { get; set; }
    }
}
