using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CCMS.Server.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly IConfigUtilService _configUtil;
        private readonly ReaderWriterLock _locker;
        public LoggingService(IConfigUtilService configUtil)
        {
            _configUtil = configUtil;
            _locker = new ReaderWriterLock();
        }

        private static string GetExceptionString(Exception exception)
        {
            return $"{exception.Message}\n{exception.StackTrace}\n{exception.Data}";
        }

        private string GetLogFilePath()
        {
            string logFolder = _configUtil.GetLogFolder();
            if (Directory.Exists(logFolder) == false)
            {
                Directory.CreateDirectory(logFolder);
            }
            string filename = "ccms_" + DateTime.Today.ToString("dd-MMM-yyyy") + ".log";
            return Path.Combine(logFolder, filename);
        }

        private void Log(string message)
        {
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
            if (ignoreLogLevels.Contains(_configUtil.GetLogLevel()))
            {
                return;
            }
            Log(message);
        }

        public async Task LogDebugAsync(string message)
        {
            string[] ignoreLogLevels = new string[] { "Error", "Information" };
            if (ignoreLogLevels.Contains(_configUtil.GetLogLevel()))
            {
                return;
            }
            await LogAsync(message);
        }

        public void LogInformation(string message)
        {
            if (_configUtil.GetLogLevel() == "Error")
            {
                return;
            }
            Log(message);
        }

        public async Task LogInformationAsync(string message)
        {
            if (_configUtil.GetLogLevel() == "Error")
            {
                return;
            }
            await LogAsync(message);
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
