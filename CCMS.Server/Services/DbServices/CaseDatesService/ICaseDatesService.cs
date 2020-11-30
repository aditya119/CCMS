using CCMS.Shared.Models;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICaseDatesService
    {
        Task<CaseDatesModel> RetrieveAsync(int caseId);
        Task UpdateAsync(CaseDatesModel caseDatesModel, int currUser);
    }
}