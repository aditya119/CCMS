namespace CCMS.Shared.Models
{
    public class ProceedingDecisionModel
    {
        public int ProceedingDecisionId { get; set; }
        public string ProceedingDecisionName { get; set; }
        public bool HasNextHearingDate { get; set; }
        public bool HasOrderAttachment { get; set; }
    }
}
