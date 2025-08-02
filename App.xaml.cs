using AqbaApp.Core;
using AqbaApp.Core.Api;
using AqbaApp.HostedServices;
using AqbaApp.Interfaces.Api;
using AqbaApp.Interfaces.Service;
using AqbaApp.Model.Client;
using AqbaApp.Service.Client;
using AqbaApp.Service.OkdeskEntity;
using AqbaApp.View;
using AqbaApp.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Windows;

namespace AqbaApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        private readonly string logDirectory;
        private readonly string logFilePath;

        public App()
        {
            logDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                "AqbaApp", 
                "logs");

            logFilePath = Path.Combine(logDirectory, $"{DateTime.Now:dd.MM.yyyy}_log.txt");

            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(
                    logFilePath, 
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss:fff}] [{Level:u4}] - {Message:lj} {NewLine}{Exception}"
                )
                .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();

            _host = Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .UseSerilog()
                .Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Singleton
            services.AddSingleton<Immutable>();
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<SettingService<MainSettings>>>();
                var immutable = provider.GetRequiredService<Immutable>();
                return new SettingService<MainSettings>(immutable.NameMainConfig, immutable.NameFolderForConfigs, logger, immutable);
            });
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<SettingService<EntitiesCache>>>();
                var immutable = provider.GetRequiredService<Immutable>();
                return new SettingService<EntitiesCache>(immutable.NameCacheFile, immutable.NameFolderForCache, logger, immutable);
            });
            services.AddSingleton(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<LogArchiver>>();
                return new LogArchiver(logDirectory, logger);
            });
            services.AddSingleton<INavigationService, NavigationService>();

            // BackgroundServices
            services.AddHostedService<LogArchiverBackgroundService>();

            // View
            services.AddTransient<AuthorizationWindow>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<AccessPage>();
            services.AddSingleton<ReportPage>();
            services.AddSingleton<SettingsPage>();

            // ViewModel
            services.AddTransient<AuthorizationViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<AccessViewModel>();
            services.AddSingleton<ReportViewModel>();
            services.AddSingleton<SettingsViewModel>();

            // Services
            services.AddHttpClient<IRequestService, RequestClient>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _host.Start();

                var logArchiver = _host.Services.GetRequiredService<LogArchiver>();
                logArchiver.ArchiveAndCleanLogs();

                var mainWindow = _host.Services.GetRequiredService<MainWindow>();
                mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                LogError(ex);
                System.Windows.MessageBox.Show("Произошла критическая ошибка, приложение будет закрыто.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
            }

            Log.Information("The application has been launched.");

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("The application is terminating.");

            try
            {
                // Сохранение настроек в файле перед закрытием приложения
                var mainSettings = _host.Services.GetRequiredService<SettingService<MainSettings>>();
                var cacheSettings = _host.Services.GetRequiredService<SettingService<EntitiesCache>>();
                mainSettings.SaveSettings();
                cacheSettings.SaveSettings();

                using (_host)
                {
                    _host.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            finally
            {
                Log.CloseAndFlush();
                base.OnExit(e);
            }
        }

        private void LogError(Exception ex)
        {
            // Чтобы не выходила ошибка, что файл для логов уже используется
            Log.CloseAndFlush();

            try
            {
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                File.AppendAllText(logFilePath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss:fff}] [FATAL] - {ex}\n");
            }
            catch
            {
                // Игнорируем ошибки логирования, чтобы не вызывать рекурсивные исключения
            }
        }
    }
}
