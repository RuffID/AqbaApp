using AqbaApp.Helper;
using Notifications.Wpf.Core;
using Serilog;

namespace AqbaApp
{
    public static class WriteLog
    {
        static WriteLog()
        {
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()                
                .WriteTo.File(Immutable.logPath, rollingInterval : RollingInterval.Day,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss:fff}] [{Level:u4}] - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }

        public static void Error(string exception)
        {
            _ = Notice.Show(NotificationType.Error, exception);
            Log.Error(exception);
        }

        public static void Warn(string message)
        {
            Log.Warning(message);
        }

        public static void Info(string message)
        {
            Log.Information(message);
        }
    }
}
