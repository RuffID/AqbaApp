using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using AqbaApp.Model.Client;
using Microsoft.Extensions.Logging;
using AqbaApp.Interfaces.Api;

namespace AqbaApp.Service.Client
{
    public class UpdateService(SettingService<MainSettings> settings, Immutable immutable, IRequestService request, ILoggerFactory logger)
    {
        private readonly ILogger<UpdateService> _logger = logger.CreateLogger<UpdateService>();
        private readonly IRequestService _request = request;
        private readonly Immutable _immutable = immutable;
        private readonly SettingService<MainSettings> _settings = settings;
        private AppInfo? AppInfo { get; set; }

        public async Task<bool> CheckUpdate()
        {
            if (!await UpdateAppInfo($"{_settings.Settings.ServerAddress}/update/version") || AppInfo == null)
                return false;

            // Если текущая версия новее или такая же как актуальная, то возращает true, иначе производит обновление
            if (Convert.ToDouble(_immutable.CurrentVersion, CultureInfo.InvariantCulture) >= Convert.ToDouble(AppInfo.Version, CultureInfo.InvariantCulture))
                return true;

            View.MessageBox.Show("Предупреждение", $"Найдено обновление {AppInfo.Version}, приложение перезапустится.\n\nОписание: {AppInfo.Description}", MessageBoxButton.OK);
            try
            {
                if (!await DownloadFile($"{_settings.Settings.ServerAddress}/update", _immutable.PathToTempUpdateFile))
                    return false;

                CmdStart(string.Format("taskkill /f /im \"{0}\" && timeout /t 1 && del \"{1}\" && move /Y \"{2}\" \"{3}\" && \"{4}\"",
                    _immutable.ApplicationFileName, _immutable.PathToApplication, _immutable.PathToTempUpdateFile, _immutable.ApplicationFileName, _immutable.ApplicationFileName));

                /* 1е - закрывает приложение по имени файла
                 * 2е - делает таймаут 1 секунду чтобы программа точно закрылась
                 * 3е - удаление файла по пути до текущего файла
                 * 4е - переносит скачанный файл из аппдаты в текущую папку
                 * 5е - запуск приложения*/

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while get updates.");
                return false;
            }
        }

        private async Task<bool> UpdateAppInfo(string linkVersion)
        {
            var response = await _request.SendGet(linkVersion);

            if (string.IsNullOrEmpty(response))
                return false;

            try
            {
                AppInfo = JsonConvert.DeserializeObject<AppInfo>(response);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while update app info.");
                return false;
            }
        }

        private async Task<bool> DownloadFile(string link, string filePath)
        {
            Stream? fileStream = await _request.SendGetStream(link);

            if (fileStream == null || fileStream == Stream.Null)
                return false;

            Directory.CreateDirectory(_immutable.PathToAppDataProjectFolder);

            using FileStream outputFileStream = new(filePath, FileMode.CreateNew);
            await fileStream.CopyToAsync(outputFileStream);
            return true;
        }

        private void CmdStart(string line)
        {
            var startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/c {line}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(startInfo);
        }
    }
}
