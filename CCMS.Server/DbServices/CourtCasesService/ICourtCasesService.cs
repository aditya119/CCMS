using CCMS.Shared.Models.CourtCaseModels;
using System;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface ICourtCasesService
    {
        Task<int> CreateAsync(NewCaseModel caseModel, int currUser);
        Task DeleteAsync(int caseId);
        Task<(string, int, DateTime?)> ExistsCaseIdAsync(int caseId);
        Task<CaseDetailsModel> RetrieveAsync(int caseId);
        Task<(int, int)> SearchCaseNumberAsync(string caseNumber);
        Task UpdateAsync(UpdateCaseModel caseModel, int currUser);
    }
}