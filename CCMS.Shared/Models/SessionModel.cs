using System;
using System.Diagnostics.CodeAnalysis;

namespace CCMS.Shared.Models
{
    public class SessionModel : IEquatable<SessionModel>
    {
        public int UserId { get; set; }
        public int PlatformId { get; set; }
        public string Guid { get; set; }

        public bool Equals(SessionModel other)
        {
            if (other is null)
            {
                return false;
            }
            bool isEqual = UserId == other.UserId && PlatformId == other.PlatformId && Guid == other.Guid;
            return isEqual;
        }
    }
}
