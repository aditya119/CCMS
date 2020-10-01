using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CourtCaseModels
{
    public class NewCaseModel
    {
        [Required]
        [StringLength(1000, ErrorMessage = "Case Number too long")]
        public string CaseNumber { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int AppealNumber { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CaseTypeId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CourtId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int LocationId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int LawyerId { get; set; }
    }
}
