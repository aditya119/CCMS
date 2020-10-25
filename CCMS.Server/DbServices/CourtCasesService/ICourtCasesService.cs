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
        Task<(int, DateTime?)> ExistsCaseNumberAsync(string caseNumber, int appealNumber);
        Task<CaseStatusModel> GetCaseStatus(int caseId);
        Task<CaseDetailsModel> RetrieveAsync(int caseId);
        Task UpdateAsync(UpdateCaseModel caseModel, int currUser);
    }
}