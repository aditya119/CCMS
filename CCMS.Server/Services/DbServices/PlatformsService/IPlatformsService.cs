using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CCMS.Server.Services.DbServices
{
    public interface IPlatformsService
    {
        Task<IEnumerable<PlatformModel>> RetrieveAllAsync();
    }
}