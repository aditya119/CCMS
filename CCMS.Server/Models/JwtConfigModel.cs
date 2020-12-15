namespace CCMS.Server.Models
{
    public class JwtConfigModel
    {
        public string Key { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int ExpiryInDays { get; init; }
    }
}
