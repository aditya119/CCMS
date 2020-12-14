using CCMS.Shared.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [StringLength(1000, ErrorMessage = "E-mail Address too long")]
        public string UserEmail { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Password too long")]
        public string Password { get; set; }
        
        [Required]
        [Range(Platforms.MinValue, Platforms.MaxValue)]
        public int PlatformId { get; set; }
    }
}