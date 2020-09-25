using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CaseTypeModels
{
    public class NewCaseTypeModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Case Type name too long")]
        public string CaseTypeName { get; set; }
    }
}
