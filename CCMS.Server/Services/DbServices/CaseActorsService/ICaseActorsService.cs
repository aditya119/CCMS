using CCMS.Shared.Models.CaseActorModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICaseActorsService
    {
        Task<IEnumerable<CaseActorModel>> RetrieveAsync(int caseId);
        Task UpdateAsync(IEnumerable<UpdateCaseActorModel> caseActorModels, int currUser);
    }
}