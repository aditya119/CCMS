using CCMS.Shared.Models.LocationModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface ILocationsService
    {
        Task<int> CreateAsync(NewLocationModel locationModel);
        Task DeleteAsync(int locationId);
        Task<IEnumerable<LocationDetailsModel>> RetrieveAllAsync();
        Task<LocationDetailsModel> RetrieveAsync(int locationId);
        Task UpdateAsync(LocationDetailsModel locationModel);
    }
}