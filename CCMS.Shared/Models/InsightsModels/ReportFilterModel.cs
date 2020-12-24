using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CCMS.Shared.Models.InsightsModels
{
    public class ReportFilterModel
    {
        [Required]
        [DefaultValue("-1")]
        public string CaseNumber { get; set; } = "-1";

        [Required]
        [Range(-1, int.MaxValue)]
        [DefaultValue(-1)]
        public int LocationId { get; set; } = -1;

        [Required]
        [Range(-1, int.MaxValue)]
        [DefaultValue(-1)]
        public int LawyerId { get; set; } = -1;

        [Required]
        [Range(-1, int.MaxValue)]
        [DefaultValue(-1)]
        public int CourtId { get; set; } = -1;

        [Required]
        public DateTime ProceedingDateRangeStart { get; set; } = DateTime.Today;

        [Required]
        public DateTime ProceedingDateRangeEnd { get; set; } = DateTime.Today.AddDays(7);
        public string Csv
        {
            get
            {
                return $"{CaseNumber},{LocationId},{LawyerId},{CourtId}," +
                    $"{ProceedingDateRangeStart:dd-MMM-yyyy},{ProceedingDateRangeEnd:dd-MMM-yyyy}";
            }
        }
    }
}
