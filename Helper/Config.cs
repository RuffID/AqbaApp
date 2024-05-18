using AqbaApp.Interfaces;
using AqbaApp.Model.Client;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace AqbaApp
{
    public static class Config
    {
        //TODO исправить на dependency injection
        public static Settings Settings;
        public static Cache Cache;

        static Config()
        {
            Settings = new();
            Cache = new();
            InitializeConfig(ref Settings);
            InitializeConfig(ref Cache);
        }

        static void Load<T>(ref T config)
        {
            try
            {
                string configString = File.ReadAllText(((IConfig)config).PathToFile);
                if (!string.IsNullOrEmpty(configString))
                    config = JsonConvert.DeserializeObject<T>(configString);

                if (config == null || (string.IsNullOrEmpty(configString) || configString == "[]"))
                    WriteLog.Error("Failed to load config");
            }
            catch (Exception ex) { WriteLog.Error(ex.ToString()); }
        }

        public static void InitializeConfig<T>(ref T config)
        {
            if (!File.Exists(((IConfig)config).PathToFile))
                SaveOrCreateConfig(ref config);
            // После в любом случае нужно загрузить настройки методом Load()
            Load(ref config);
        }

        public static void SaveOrCreateConfig<T>(ref T config)
        {
            try
            {
                string json = JsonConvert.SerializeObject(config, ((IConfig)config).Formatting);

                if (!Directory.Exists(Path.Combine(Immutable.appdataPath, ((IConfig)config).PathToFolder)))
                    Directory.CreateDirectory(Path.Combine(Immutable.appdataPath, ((IConfig)config).PathToFolder));

                File.WriteAllText(((IConfig)config).PathToFile, json, Encoding.UTF8);
            }
            catch (Exception ex) { WriteLog.Error(ex.ToString()); }
        }
    }
}