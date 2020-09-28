using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CourtModels
{
    public class CourtDetailsModel : NewCourtModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int CourtId { get; set; }
    }
}
