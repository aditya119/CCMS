using CCMS.Shared.Models.CourtModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ICourtsService
    {
        Task<int> CreateAsync(NewCourtModel courtModel);
        Task DeleteAsync(int courtId);
        Task<IEnumerable<CourtDetailsModel>> RetrieveAllAsync();
        Task<CourtDetailsModel> RetrieveAsync(int courtId);
        Task UpdateAsync(CourtDetailsModel courtModel);
    }
}