using System;

namespace CCMS.Shared.Models.CaseProceedingModels
{
    public class CaseProceedingModel : UpdateCaseProceedingModel
    {
        public int CaseId { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
