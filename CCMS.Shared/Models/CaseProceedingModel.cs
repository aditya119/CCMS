using CCMS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models
{
    public class CaseProceedingModel : IValidatableObject
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CaseProceedingId { get; set; }
        public DateTime ProceedingDate { get; set; }

        [Required]
        [Range(ProceedingDecisions.MinValue, ProceedingDecisions.MaxValue)]
        public int ProceedingDecision { get; set; }
        public DateTime? NextHearingOn { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int JudgementFile { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (NextHearingOn.HasValue)
            {
                if (ProceedingDate > NextHearingOn.Value)
                {
                    results.Add(new ValidationResult("Proceeding Date cannot be later than Next Hearing date",
                        new List<string> { "NextHearingOn" }));
                }
            }
            return results;
        }
    }
}
