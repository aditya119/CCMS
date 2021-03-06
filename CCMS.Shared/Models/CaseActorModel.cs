﻿using CCMS.Shared.Enums;
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
        [Range(CaseActors.MinValue, CaseActors.MaxValue)]
        public int ActorTypeId { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Name too long")]
        public string ActorName { get; set; }

        [StringLength(4000, ErrorMessage = "Address too long")]
        public string ActorAddress { get; set; }

        [EmailAddress]
        [StringLength(1000, ErrorMessage = "E-mail Address too long")]
        public string ActorEmail { get; set; }

        [StringLength(1000, ErrorMessage = "Phone Number too long")]
        public string ActorPhone { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int DetailFile { get; set; }
    }
}
