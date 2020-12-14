using CCMS.Server.Models;
using System.Collections.Generic;

namespace CCMS.Server.Services
{
    public interface IConfigUtilService
    {
        IEnumerable<string> GetAllowedExtensions();
        JwtConfigModel GetJwtConfig();
        string GetLogFolder();
        string GetLogLevel();
    }
}