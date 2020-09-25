using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.AppUserModels
{
    public class UserDetailsModel : NewUserModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
    }
}
