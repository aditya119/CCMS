using CCMS.Shared.Models;
using System.Collections.Generic;
using System.Linq;

namespace CCMS.Server.Utilities
{
    public class SessionCacheUtil
    {
        private static List<SessionModel> sessions;
        public static void AddSessionToCache(SessionModel sessionModel)
        {
            if (sessions == null)
            {
                sessions = new List<SessionModel>();
            }
            SessionModel existingModel = sessions.FirstOrDefault(s => s.UserId == sessionModel.UserId && s.PlatformId == sessionModel.PlatformId);
            if (existingModel != null)
            {
                ClearSessionFromCache(existingModel);
            }
            if (sessions.Count > 1000)
            { // store only 1000 sessions in cache, use db for more than that
                return;
            }
            sessions.Add(sessionModel);
        }
        public static bool ExistsSessionInCache(SessionModel sessionModel)
        {
            if (sessions == null)
            {
                sessions = new List<SessionModel>();
                return false;
            }
            return sessions.Contains(sessionModel);
        }
        public static void ClearSessionFromCache(SessionModel sessionModel)
        {
            if (ExistsSessionInCache(sessionModel))
            {
                sessions.Remove(sessionModel);
            }
        }
    }
}
