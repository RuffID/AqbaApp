using System.Reflection;
using System;
using System.IO;

namespace AqbaApp
{
    public static class Immutable
    {
        public static readonly string appdataPath;
        public static readonly string appPath;    // Путь до файла приложения
        public static readonly string appName;  // Имя приложения
        public static readonly string tempUpdFilePath; // Путь до временного файла куда скачивается приложение при обновлении
        public static readonly string curVer;
        public static readonly string configFolderName;
        public static readonly string configPath;
        public static readonly string cacheFolderName;
        public static readonly string cachePath;
        public static readonly string logPath;

        static Immutable()
        {
            appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly().GetName().Name);
            appPath = Environment.ProcessPath;
            appName = Path.GetFileName(appPath);            
            tempUpdFilePath = Path.Combine(appdataPath, "AqbaApp.exe");
            curVer = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            configFolderName = "Config";
            cacheFolderName = "EntitiesCache";
            configPath = Path.Combine(appdataPath, configFolderName, "config.json");
            cachePath = Path.Combine(appdataPath, cacheFolderName, "entities_cache.json");
            logPath = Path.Combine(appdataPath, "Logs", "log_.txt");
        }
    }
}
