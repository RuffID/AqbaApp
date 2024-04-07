using AqbaApp.Model.Authorization;
using AqbaApp.Model.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AqbaApp
{
    public static class Config
    {
        public static Settings Settings { get; set; }

        static Config()
        {
            Settings = new();
            InitializeConfig();
        }

        static void Load()
        {
            try
            {
                string configString = File.ReadAllText(Immutable.configPath);
                if (!string.IsNullOrEmpty(configString))
                    Settings = JsonConvert.DeserializeObject<Settings>(configString);

                if (Settings == null || (string.IsNullOrEmpty(configString) || configString == "[]"))
                    WriteLog.Error("Failed to load config");
            }
            catch (Exception ex) { WriteLog.Error(ex.ToString()); }
        }

        public static void InitializeConfig()
        {
            if (!File.Exists(Immutable.configPath))
                SaveOrCreateConfig();
            // После в любом случае нужно загрузить настройки методом Load()
            Load();
        }

        public static void SaveOrCreateConfig()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);

                if (!Directory.Exists(Path.Combine(Immutable.appdataPath, "Config")))
                    Directory.CreateDirectory(Path.Combine(Immutable.appdataPath, "Config"));
                               
                File.WriteAllText(Immutable.configPath, json, Encoding.UTF8);
            }
            catch (Exception ex) { WriteLog.Error(ex.ToString()); }
        }

        public static void SaveToken(AuthenticateResponse response)
        {            
            Settings.Token = response;
            SaveOrCreateConfig();
        }
    }
}