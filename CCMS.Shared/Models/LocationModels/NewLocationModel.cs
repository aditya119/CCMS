using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.LocationModels
{
    public class NewLocationModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Location name too long")]
        public string LocationName { get; set; }
    }
}
