using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IRolesService
    {
        Task<int> GetRoleIdAsync(string rolesCsv);
        Task<IEnumerable<RoleModel>> RetrieveAllAsync();
    }
}