using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.LocationModels
{
    public class LocationDetailsModel : NewLocationModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int LocationId { get; set; }
    }
}
