using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.LawyerModels
{
    public class LawyerDetailsModel : NewLawyerModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int LawyerId { get; set; }
    }
}