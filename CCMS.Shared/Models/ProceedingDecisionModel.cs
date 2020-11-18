namespace CCMS.Shared.Models
{
    public class ProceedingDecisionModel
    {
        public int ProceedingDecisionId { get; init; }
        public string ProceedingDecisionName { get; init; }
        public bool HasNextHearingDate { get; init; }
        public bool HasOrderAttachment { get; init; }
    }
}
