namespace CCMS.Shared.Models
{
    public class JwtDetailsModel
    {
        public JwtDetailsModel(int userId, string userEmail, int platformId, string rolesCsv, string guid)
        {
            UserId = userId;
            RolesCsv = rolesCsv;
            Guid = guid;
            UserEmail = userEmail;
            PlatformId = platformId;
        }
        public int UserId { get; set; }
        public string UserEmail { get; set; }
        public int PlatformId { get; set; }
        public string Guid { get; set; }
        public string RolesCsv { get; set; }
        public string[] RolesArray
        {
            get
            {
                if (string.IsNullOrWhiteSpace(RolesCsv))
                {
                    return null;
                }
                return RolesCsv.Split(',');
            }
        }
    }
}
