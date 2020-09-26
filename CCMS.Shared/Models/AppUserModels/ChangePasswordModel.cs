using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CCMS.Shared.Models.AppUserModels
{
    public class ChangePasswordModel : IValidatableObject
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Password too long")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string NewPasswordAgain { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (NewPassword.Equals(NewPasswordAgain) == false)
            {
                results.Add(new ValidationResult("New passwords must match",
                    new List<string> { "NewPassword", "NewPasswordAgain" }));
            }
            if (NewPassword.Any(char.IsUpper) == false)
            {
                results.Add(new ValidationResult("Password must contain at least one upper-case letter",
                    new List<string> { "NewPassword" }));
            }
            if (NewPassword.Any(char.IsLower) == false)
            {
                results.Add(new ValidationResult("Password must contain at least one lower-case letter",
                    new List<string> { "NewPassword" }));
            }
            if (NewPassword.Any(char.IsDigit) == false)
            {
                results.Add(new ValidationResult("Password must contain at least one digit",
                    new List<string> { "NewPassword" }));
            }
            return results;
        }
    }
}
