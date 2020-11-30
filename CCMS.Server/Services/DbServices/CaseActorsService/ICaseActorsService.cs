using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICaseActorsService
    {
        Task<IEnumerable<CaseActorModel>> RetrieveAsync(int caseId);
        Task UpdateAsync(IEnumerable<CaseActorModel> caseActorModels, int currUser);
    }
}