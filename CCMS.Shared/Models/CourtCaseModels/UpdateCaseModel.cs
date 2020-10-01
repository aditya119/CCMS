using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CourtCaseModels
{
    public class UpdateCaseModel : NewCaseModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CaseId { get; set; }
    }
}
