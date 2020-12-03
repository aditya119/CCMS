using CCMS.Server.Models;

namespace CCMS.Server.Services
{
    public interface ICryptoService
    {
        string GenerateJSONWebToken(JwtDetailsModel jwtDetailsModel);
        string GenerateRandomSalt();
        string SaltAndHashText(string text, string salt);
    }
}