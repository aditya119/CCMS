using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICaseProceedingsService
    {
        Task AssignProceedingAsync(int caseProceedingId, int assignTo, int currUser);
        Task DeleteAsync(int caseProceedingId, int currUser);
        Task<IEnumerable<CaseProceedingModel>> RetrieveAllCaseProceedingsAsync(int caseId);
        Task<CaseProceedingModel> RetrieveAsync(int caseProceedingId);
        Task<IEnumerable<PendingProceedingModel>> RetrievePendingProceedingsAsync(int userId);
        Task UpdateAsync(CaseProceedingModel caseProceedingModel, int currUser);
    }
}