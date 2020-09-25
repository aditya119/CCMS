using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.LawyerModels
{
    public class LawyerDetailsModel : NewLawyerModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int LawyerId { get; set; }
    }
}