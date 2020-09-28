using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CaseTypeModels
{
    public class CaseTypeDetailsModel : NewCaseTypeModel
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int CaseTypeId { get; set; }
    }
}
