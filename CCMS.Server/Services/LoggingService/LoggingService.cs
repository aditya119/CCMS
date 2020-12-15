using CCMS.Server.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CCMS.Server.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly LogConfigModel _logConfig;
        private readonly ReaderWriterLock _locker;

        public LoggingService(LogConfigModel logConfig)
        {
            _logConfig = logConfig;
            _locker = new ReaderWriterLock();
        }

        private static string GetExceptionString(Exception exception)
        {
            return $"[ERROR]: {exception.Message}\n{exception.StackTrace}\n{exception.Data}";
        }

        private string GetLogFilePath()
        {
            if (Directory.Exists(_logConfig.LogFolder) == false)
            {
                Directory.CreateDirectory(_logConfig.LogFolder);
            }
            string filename = "ccms_" + DateTime.Today.ToString("dd-MMM-yyyy") + ".log";
            return Path.Combine(_logConfig.LogFolder, filename);
        }

        private void Log(string message)
        {
            if (_logConfig.LogFolder == "@Console")
            {
                Console.WriteLine(message);
                return;
            }
            string logFilePath = GetLogFilePath();
            message = $"{DateTime.Today:dd-MMM-yyyy}: {message}";
            try
            {
                _locker.AcquireReaderLock(1000);
                if (File.Exists(logFilePath) == false)
                {
                    using StreamWriter sw = File.CreateText(logFilePath);
                    sw.WriteLine(message);
                }
                else
                {
                    using StreamWriter sw = File.AppendText(logFilePath);
                    sw.WriteLine(message);
                }
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }
        }

        private async Task LogAsync(string message)
        {
            if (_logConfig.LogFolder == "@Console")
            {
                Console.WriteLine(message);
                return;
            }
            string logFilePath = GetLogFilePath();
            message = $"{DateTime.Today:dd-MMM-yyyy}: {message}";
            try
            {
                _locker.AcquireReaderLock(1000);
                if (File.Exists(logFilePath) == false)
                {
                    using StreamWriter sw = File.CreateText(logFilePath);
                    await sw.WriteLineAsync(message);
                }
                else
                {
                    using StreamWriter sw = File.AppendText(logFilePath);
                    await sw.WriteLineAsync(message);
                }
            }
            finally
            {
                _locker.ReleaseWriterLock();
            }
        }

        public void LogDebug(string message)
        {
            string[] ignoreLogLevels = new string[] { "Error", "Information" };
            if (ignoreLogLevels.Contains(_logConfig.LogLevel))
            {
                return;
            }
            Log($"[DEBUG]: {message}");
        }

        public async Task LogDebugAsync(string message)
        {
            string[] ignoreLogLevels = new string[] { "Error", "Information" };
            if (ignoreLogLevels.Contains(_logConfig.LogLevel))
            {
                return;
            }
            await LogAsync($"[DEBUG]: {message}");
        }

        public void LogInformation(string message)
        {
            if (_logConfig.LogLevel == "Error")
            {
                return;
            }
            Log($"[INFO]: {message}");
        }

        public async Task LogInformationAsync(string message)
        {
            if (_logConfig.LogLevel == "Error")
            {
                return;
            }
            await LogAsync($"[INFO]: {message}");
        }

        public void LogError(Exception exception)
        {
            string message = GetExceptionString(exception);
            Log(message);
        }

        public async Task LogErrorAsync(Exception exception)
        {
            string message = GetExceptionString(exception);
            await LogAsync(message);
        }
    }
}
