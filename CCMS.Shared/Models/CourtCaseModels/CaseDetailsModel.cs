using System;

namespace CCMS.Shared.Models.CourtCaseModels
{
    public class CaseDetailsModel : UpdateCaseModel
    {
        public int CaseStatus { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
