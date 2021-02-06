namespace CCMS.Web.Models
{
    public class CaseIdentityModel
    {
        public CaseIdentityModel(int caseId, string caseNumber, int appealNumber)
        {
            CaseId = caseId;
            CaseNumber = caseNumber;
            AppealNumber = appealNumber;
        }
        public int CaseId { get; set; }
        public string CaseNumber { get; set; }
        public int AppealNumber { get; set; }
    }
}
