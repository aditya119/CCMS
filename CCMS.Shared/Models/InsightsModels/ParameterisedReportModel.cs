using System;

namespace CCMS.Shared.Models.InsightsModels
{
    public class ParameterisedReportModel
    {
        public int CaseId { get; set; }
        public string CaseNumber { get; set; }
        public int AppealNumber { get; set; }
        public string CourtName { get; set; }
        public string LocationName { get; set; }
        public int CaseFiledOnYear { get; set; }
        public string LawyerFullname { get; set; }
        public DateTime ProceedingDate { get; set; }
        public string ProceedingDecision { get; set; }
        public DateTime NextHearingOn { get; set; }
    }
}
