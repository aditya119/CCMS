using CCMS.Shared.Models.CaseProceedingModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICaseProceedingsService
    {
        Task AssignProceedingAsync(int caseProceedingId, int assignTo, int currUser);
        Task<IEnumerable<CaseProceedingModel>> RetrieveAllCaseProceedingsAsync(int caseId);
        Task<CaseProceedingModel> RetrieveAsync(int caseProceedingId);
        Task UpdateAsync(UpdateCaseProceedingModel caseProceedingModel, int currUser);
    }
}