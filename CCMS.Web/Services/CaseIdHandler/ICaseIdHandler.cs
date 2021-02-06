using CCMS.Web.Models;
using System.Threading.Tasks;

namespace CCMS.Web.Services
{
    public interface ICaseIdHandler
    {
        Task<CaseIdentityModel> RetrieveCaseDetails();
        Task StoreCaseDetails(CaseIdentityModel caseIdentity);
    }
}