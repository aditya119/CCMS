using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IRolesService
    {
        Task<int> GetRoleIdAsync(string rolesCsv);
        Task<IEnumerable<RoleModel>> RetrieveAllAsync();
    }
}