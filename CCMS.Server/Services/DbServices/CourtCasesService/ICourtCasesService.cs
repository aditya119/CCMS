using CCMS.Shared.Models.CourtCaseModels;
using System;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICourtCasesService
    {
        Task<int> CreateAsync(NewCaseModel caseModel, int currUser);
        Task DeleteAsync(int caseId, int currUser);
        Task<(string caseNumber, int appealNumber, DateTime? deleted)> ExistsCaseIdAsync(int caseId);
        Task<(int caseId, DateTime? deleted)> ExistsCaseNumberAsync(string caseNumber, int appealNumber);
        Task<CaseStatusModel> GetCaseStatusAsync(int caseId);
        Task<CaseDetailsModel> RetrieveAsync(int caseId);
        Task UpdateAsync(UpdateCaseModel caseModel, int currUser);
    }
}