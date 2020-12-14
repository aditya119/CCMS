namespace CCMS.Server.Models
{
    public class JwtConfigModel
    {
        public JwtConfigModel(string key, string issuer, string audience, string expiryInDays)
        {
            Key = key;
            Issuer = issuer;
            Audience = audience;
            ExpiryInDays = int.Parse(expiryInDays);
        }
        public string Key { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int ExpiryInDays { get; init; }
    }
}
