using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IProceedingDecisionsService
    {
        Task<IEnumerable<ProceedingDecisionModel>> RetrieveAllAsync();
    }
}