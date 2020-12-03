namespace CCMS.Server.Models
{
    public class SessionModel
    {
        public SessionModel(int userId, int platformId, string guid)
        {
            UserId = userId;
            PlatformId = platformId;
            Guid = guid;
        }
        public int UserId { get; init; }
        public int PlatformId { get; init; }
        public string Guid { get; init; }
    }
}
