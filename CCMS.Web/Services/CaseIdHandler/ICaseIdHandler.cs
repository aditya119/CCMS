using CCMS.Web.Models;
using System.Threading.Tasks;

namespace CCMS.Web.Services
{
    public interface ICaseIdHandler
    {
        Task<CaseIdentityModel> RetrieveCaseDetailsAsync();
        Task StoreCaseDetailsAsync(CaseIdentityModel caseIdentity);
    }
}