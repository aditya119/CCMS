using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IRolesService
    {
        Task<IEnumerable<RoleModel>> GetAllRoles();
        Task<int> GetRoleId(string rolesCsv);
    }
}