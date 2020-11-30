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
        Task<IEnumerable<AssignedProceedingModel>> RetrieveAssignedProceedingsAsync(int userId);
        Task<CaseProceedingModel> RetrieveAsync(int caseProceedingId);
        Task UpdateAsync(CaseProceedingModel caseProceedingModel, int currUser);
    }
}