using CCMS.Shared.Models.CaseActorsModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface ICaseActorsService
    {
        Task<IEnumerable<CaseActorModel>> RetrieveAsync(int caseId);
        Task UpdateAsync(IEnumerable<UpdateCaseActorModel> caseActorModels, int currUser);
    }
}