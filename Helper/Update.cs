using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using System.Windows.Input;
using System.Windows;
using Newtonsoft.Json;
using System.Diagnostics;
using AqbaApp.API;
using System.IO;
using System.Net.Http;
using AqbaApp.Model.Server;

namespace AqbaApp.Helper
{
    public static class Update
    {

        static AppInfo appInfo;
        static string linkDownload;
        static string linkVersion;
        static readonly SocketsHttpHandler socketsHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };

        static Update()
        {
            linkDownload = $"{Config.Settings.ServerAddress}/update";
            linkVersion = $"{Config.Settings.ServerAddress}/update/version";
        }

        public static async Task<bool> CheckUpdate()
        {
            await UpdateAppInfo(linkVersion);
            if (appInfo == null)
            {
                WriteLog.Warn("Failed to get application version");
                return false;
            }

            if (Convert.ToDouble(Immutable.curVer, CultureInfo.InvariantCulture) < Convert.ToDouble(AppInfo.Version, CultureInfo.InvariantCulture))
            {
                View.MessageBox.Show("Предупреждение", $"Найдено обновление {AppInfo.Version}, приложение перезапустится.\n\nОписание: {AppInfo.Description}", MessageBoxButton.OK);
                try
                {
                    if (!await DownloadFile(linkDownload, Immutable.tempUpdFilePath))
                        return false;

                    Cmd($"taskkill /f /im \"{Immutable.appName}\" && timeout /t 1 && del \"{Immutable.appPath}\" && move /Y \"{Immutable.tempUpdFilePath}\" \"{Immutable.appName}\" && \"{Immutable.appName}\"");
                    return true;
                    /*1е - закрывает приложение по имени файла
                     *2е - делает таймаут 1 секунду чтобы программа точно закрылась
                     * 3е - удаление файла по пути до текущего файла
                     *4е - переносит скачанный файл из аппдаты в текущую папку
                     * 5е - запуск приложения*/
                }
                catch (Exception ex) { WriteLog.Error(ex.ToString()); return false; }
            }
            return true;
        }

        static async Task UpdateAppInfo(string linkVersion)
        {
            try
            {
                var info = await GetResponse(linkVersion);
                if (!string.IsNullOrEmpty(info))
                    appInfo = JsonConvert.DeserializeObject<AppInfo>(info);
            }
            catch (Exception e) { WriteLog.Error(e.ToString()); }
        }

        static async Task<bool> DownloadFile(string link, string filePath)
        {

            Stream fileStream = await Request.GetFileStream(link);

            if (fileStream != Stream.Null)
            {
                await SaveStream(fileStream, Immutable.appdataPath, filePath);
                return true;
            }
            return false;                        
        }

        static async Task SaveStream(Stream fileStream, string directoryPath, string filePath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            using FileStream outputFileStream = new(filePath, FileMode.CreateNew);
            await fileStream.CopyToAsync(outputFileStream);
        }


        static void Cmd(string line)
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

        static async Task<string> GetResponse(string link)
        {
            try
            {
                HttpClient httpClient = new(socketsHandler);
                using HttpResponseMessage response = await httpClient.GetAsync(link);
                if (!response.IsSuccessStatusCode)
                    return null;
                else return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
        }
    }
}
