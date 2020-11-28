﻿using CCMS.Shared.Models;
using System.Threading.Tasks;

namespace CCMS.Server.DbServices
{
    public interface IAuthService
    {
        Task<(int, string, string)> FetchUserDetailsAsync(string userEmail);
        Task<bool> IsValidSessionAsync(SessionModel sessionModel);
        Task<string> LoginUserAsync(SessionModel sessionModel);
        Task LogoutAsync(int userId, int platformId);
    }
}