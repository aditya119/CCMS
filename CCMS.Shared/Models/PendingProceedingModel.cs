using System;

namespace CCMS.Shared.Models
{
    public class PendingProceedingModel
    {
        public int CaseProceedingId { get; set; }
        public string CaseNumber { get; set; }
        public int AppealNumber { get; set; }
        public string CaseStatus { get; set; }
        public DateTime ProceedingDate { get; set; }
        public DateTime NextHearingOn { get; set; }
        public string AssignedTo { get; set; }
    }
}
