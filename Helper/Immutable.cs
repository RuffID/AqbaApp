using System.Reflection;
using System;
using System.IO;

namespace AqbaApp
{
    public static class Immutable
    {
        public static readonly string jsJsonGetAcceptHeader;
        public static readonly string textGetAcceptHeader;
        public static readonly string textJsGetAcceptHeader;
        public static readonly string appdataPath;
        public static readonly string appPath;    // Путь до файла приложения
        public static readonly string appName;  // Имя приложения
        public static readonly string tempUpdFilePath; // Путь до временного файла куда скачивается приложение при обновлении
        public static readonly string curVer;
        public static readonly string configPath;
        public static readonly string logPath;

        static Immutable()
        {
            jsJsonGetAcceptHeader = "application/json, text/javascript";
            textGetAcceptHeader = "text/html";
            textJsGetAcceptHeader = "*/*;q=0.5, text/javascript, application/javascript, application/ecmascript, application/x-ecmascript";
            appdataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly().GetName().Name);
            appPath = Environment.ProcessPath;
            appName = Path.GetFileName(appPath);            
            tempUpdFilePath = Path.Combine(appdataPath, "AqbaApp.exe");
            curVer = Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            configPath = Path.Combine(appdataPath, "Config", "config.json");
            logPath = Path.Combine(appdataPath, "Logs", "log_.txt");
        }
    }
}
