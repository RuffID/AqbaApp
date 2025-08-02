using AqbaApp.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AqbaApp.HostedServices
{
    public class LogArchiverBackgroundService : BackgroundService
    {
        private readonly LogArchiver _logArchiver;
        private readonly ILogger<LogArchiverBackgroundService> _logger;

        public LogArchiverBackgroundService(LogArchiver logArchiver, ILogger<LogArchiverBackgroundService> logger)
        {
            _logArchiver = logArchiver;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("LogArchiverBackgroundService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                DateTime now = DateTime.Now;
                DateTime nextRun = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour: 0, minute: 2, second: 0).AddDays(1);
                TimeSpan delay = nextRun - now;

                try
                {
                    await Task.Delay(delay, stoppingToken);

                    if (stoppingToken.IsCancellationRequested)
                        break;

                    _logArchiver.ArchiveAndCleanLogs();
                    _logger.LogInformation("Logs for the previous day have been successfully archived.");
                }
                catch (TaskCanceledException)
                {
                    break; // Остановка сервиса - нормальная ситуация
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in LogArchiverBackgroundService.");
                }
            }

            _logger.LogInformation("LogArchiverBackgroundService has terminated.");
        }
    }
}
