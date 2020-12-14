using System;
using System.Threading.Tasks;

namespace CCMS.Server.Services
{
    public interface ILoggingService
    {
        void LogDebug(string message);
        Task LogDebugAsync(string message);
        void LogInformation(string message);
        Task LogInformationAsync(string message);
        void LogError(Exception exception);
        Task LogErrorAsync(Exception exception);
    }
}