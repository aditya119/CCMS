using Blazored.LocalStorage;
using CCMS.Web.Models;
using System.Threading.Tasks;

namespace CCMS.Web.Services
{
    public class CaseIdHandler : ICaseIdHandler
    {
        private readonly string key = "caseIdentity";
        private readonly ILocalStorageService _localStorage;

        public CaseIdHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        private async Task ClearPriorData()
        {
            if (await _localStorage.ContainKeyAsync(key))
            {
                await _localStorage.RemoveItemAsync(key);
            }
        }

        public async Task StoreCaseDetails(CaseIdentityModel caseIdentity)
        {
            await ClearPriorData();
            await _localStorage.SetItemAsync(key, caseIdentity);
        }

        public async Task<CaseIdentityModel> RetrieveCaseDetails()
        {
            var caseIdentity = await _localStorage.GetItemAsync<CaseIdentityModel>(key);
            return caseIdentity;
        }
    }
}
