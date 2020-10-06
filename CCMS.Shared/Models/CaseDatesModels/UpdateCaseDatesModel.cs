using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CaseDatesModels
{
    public class UpdateCaseDatesModel : IValidatableObject
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CaseId { get; set; }
        public DateTime? CaseFiledOn { get; set; }
        public DateTime? NoticeReceivedOn { get; set; }
        public DateTime? FirstHearingOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (CaseFiledOn.HasValue && NoticeReceivedOn.HasValue)
            {
                if (CaseFiledOn.Value > NoticeReceivedOn.Value)
                {
                    results.Add(new ValidationResult("Case cannot be filed after Notice is received",
                        new List<string> { "CaseFiledOn" }));
                }
            }
            if (CaseFiledOn.HasValue && FirstHearingOn.HasValue)
            {
                if (CaseFiledOn.Value > FirstHearingOn.Value)
                {
                    results.Add(new ValidationResult("First Hearing Date cannot be before the date Case was filed on",
                        new List<string> { "FirstHearingOn" }));
                }
            }
            if (NoticeReceivedOn.HasValue && FirstHearingOn.HasValue)
            {
                if (NoticeReceivedOn.Value > FirstHearingOn.Value)
                {
                    results.Add(new ValidationResult("First Hearing Date cannot be before the date on which Notice was received",
                        new List<string> { "NoticeReceivedOn" }));
                }
            }
            return results;
        }
    }
}
