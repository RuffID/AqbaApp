using AqbaApp.Model.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;

namespace AqbaApp.Service.Client
{
    public class SettingService<T> where T : class, new()
    {
        private readonly ILogger<SettingService<T>> _logger;
        private readonly Immutable _immutable;
        private readonly string _filePath;
        public T Settings { get; private set; }

        public SettingService(string fileName, string folderName, ILogger<SettingService<T>> logger, Immutable immutable)
        {
            _logger = logger;
            _immutable = immutable;
            Settings = new T();
            string folderPath = Path.Combine(_immutable.PathToAppDataProjectFolder, folderName);
            Directory.CreateDirectory(folderPath);
            _filePath = Path.Combine(folderPath, fileName);

            LoadSettings();
        }

        public void LoadSettings()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    Settings = JsonConvert.DeserializeObject<T>(json) ?? new T();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading settings from {FilePath}.", _filePath);
                    SaveSettings(); // Перезаписываем файл с дефолтными значениями
                }
            }
            else
            {
                Settings = new T();
                SaveSettings();
            }
        }

        public void SaveSettings()
        {
            try
            {
                string json;
                try
                {
                    json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                }
                catch (Exception serializationEx)
                {
                    _logger.LogError(serializationEx, "Error serializing settings of type {SettingsType}.", typeof(T).Name);
                    return;
                }

                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving settings to {FilePath}.", _filePath);
            }
        }
    }
}