using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CCMS.Shared.Models.AppUserModels
{
    public class UserDetailsModel
    {
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(1000, ErrorMessage = "E-mail Address too long")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Full name too long")]
        public string UserFullname { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UserRoles { get; set; }
    }
    public class UserListItemModel
    {
        public int UserId { get; set; }
        public string UserNameAndEmail { get; set; }
    }
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
                results.Add(new ValidationResult("Password must contain at lease one upper-case letter",
                    new List<string> { "NewPassword" }));
            }
            if (NewPassword.Any(char.IsLower) == false)
            {
                results.Add(new ValidationResult("Password must contain at lease one lower-case letter",
                    new List<string> { "NewPassword" }));
            }
            if (NewPassword.Any(char.IsDigit) == false)
            {
                results.Add(new ValidationResult("Password must contain at lease one digit",
                    new List<string> { "NewPassword" }));
            }
            return results;
        }
    }
}