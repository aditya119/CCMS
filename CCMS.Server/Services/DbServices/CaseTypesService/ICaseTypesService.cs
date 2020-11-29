using CCMS.Shared.Models.CaseTypeModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICaseTypesService
    {
        Task<int> CreateAsync(NewCaseTypeModel caseTypeModel);
        Task DeleteAsync(int case_typeId);
        Task<IEnumerable<CaseTypeDetailsModel>> RetrieveAllAsync();
        Task<CaseTypeDetailsModel> RetrieveAsync(int case_typeId);
        Task UpdateAsync(CaseTypeDetailsModel caseTypeModel);
    }
}