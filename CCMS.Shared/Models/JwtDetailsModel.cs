namespace CCMS.Shared.Models
{
    public class JwtDetailsModel
    {
        public JwtDetailsModel(int userId, string userEmail, int platformId, string rolesCsv, string guid)
        {
            UserId = userId;
            RolesArray = rolesCsv.Split(',');
            Guid = guid;
            UserEmail = userEmail;
            PlatformId = platformId;
        }
        public int UserId { get; init; }
        public string UserEmail { get; init; }
        public int PlatformId { get; init; }
        public string Guid { get; init; }
        public string[] RolesArray { get; init; }
    }
}
