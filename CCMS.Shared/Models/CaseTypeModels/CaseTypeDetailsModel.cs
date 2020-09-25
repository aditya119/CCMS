using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CaseTypeModels
{
    public class CaseTypeDetailsModel : NewCaseTypeModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CaseTypeId { get; set; }
    }
}
