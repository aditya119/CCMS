using CCMS.Server.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace CCMS.Server.Services
{
    public class ConfigUtilService : IConfigUtilService
    {
        private readonly IConfiguration _config;

        public ConfigUtilService(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<string> GetAllowedExtensions()
        {
            return _config.GetSection("FileUpload:AllowedExtensions").Get<List<string>>();
        }

        public JwtConfigModel GetJwtConfig()
        {
            return new JwtConfigModel(_config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"], _config["Jwt:ExpiryInDays"]);
        }
    }
}
