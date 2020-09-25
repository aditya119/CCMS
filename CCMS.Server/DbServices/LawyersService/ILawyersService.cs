using CCMS.Shared.Models.LawyerModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface ILawyersService
    {
        Task<int> CreateAsync(NewLawyerModel lawyerModel);
        Task DeleteAsync(int lawyerId);
        Task<IEnumerable<LawyerListItemModel>> RetrieveAllAsync();
        Task<LawyerDetailsModel> RetrieveAsync(int lawyerId);
        Task UpdateAsync(LawyerDetailsModel lawyerModel);
    }
}