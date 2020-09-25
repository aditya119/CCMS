using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.AppUserModels
{
    public class NewUserModel
    {
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
}
