using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models
{
    public class CaseActorModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CaseId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int ActorTypeId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Name too long")]
        public string ActorName { get; set; }

        [Required]
        [StringLength(4000, ErrorMessage = "Address too long")]
        public string ActorAddress { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(1000, ErrorMessage = "E-mail Address too long")]
        public string ActorEmail { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Phone Number too long")]
        public string ActorPhone { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int DetailFile { get; set; }
    }
}
