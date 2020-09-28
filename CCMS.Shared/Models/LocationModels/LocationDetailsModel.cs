using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.LocationModels
{
    public class LocationDetailsModel : NewLocationModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int LocationId { get; set; }
    }
}
