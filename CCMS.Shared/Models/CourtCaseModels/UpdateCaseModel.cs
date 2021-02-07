using System;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.CourtCaseModels
{
    public class UpdateCaseModel : NewCaseModel
    {
        public UpdateCaseModel() { }
        public UpdateCaseModel(NewCaseModel caseDetails, int caseId)
        {
            CaseNumber = caseDetails.CaseNumber;
            AppealNumber = caseDetails.AppealNumber;
            CaseTypeId = caseDetails.CaseTypeId;
            CourtId = caseDetails.CourtId;
            LocationId = caseDetails.LocationId;
            LawyerId = caseDetails.LawyerId;
            CaseId = caseId;
        }
        [Required]
        [Range(1, int.MaxValue)]
        public int CaseId { get; set; }
    }
}
