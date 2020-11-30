using System;

namespace CCMS.Shared.Models.InsightsModels
{
    public class ReportFilterModel
    {
        public string CaseNumber { get; set; }
        public int LocationId { get; set; }
        public int LawyerId { get; set; }
        public int CourtId { get; set; }
        public DateTime ProceedingDateRangeStart { get; set; }
        public DateTime ProceedingDateRangeEnd { get; set; }
        public string Csv
        {
            get
            {
                return $"{CaseNumber},{LocationId},{LawyerId},{CourtId}," +
                    $"{ProceedingDateRangeStart:dd-MMM-yyyy},{ProceedingDateRangeEnd::dd-MMM-yyyy}";
            }
        }
    }
}
