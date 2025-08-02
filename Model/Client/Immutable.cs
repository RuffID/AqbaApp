using System.Reflection;
using System;
using System.IO;

namespace AqbaApp.Model.Client
{
    public class Immutable
    {
        public string PathToAppDataProjectFolder { get; private set; }
        public string PathToApplication { get; private set; }
        public string ApplicationFileName { get; private set; }
        public string PathToTempUpdateFile { get; private set; }
        public string CurrentVersion { get; private set; }
        public string NameFolderForConfigs { get; private set; }
        public string NameMainConfig { get; private set; }
        public string NameFolderForCache { get; private set; }
        public string NameCacheFile { get; private set; }
        public long LimitReceivingEntitiesFromApi { get; private set; }
        public string ApiMainEndpoint { get; private set; }
        public string ApiLoginEndpoint { get; private set; }

        public Immutable()
        {
            PathToAppDataProjectFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetExecutingAssembly().GetName().Name!);
            PathToApplication = Environment.ProcessPath!;
            ApplicationFileName = Path.GetFileName(PathToApplication);
            PathToTempUpdateFile = Path.Combine(PathToAppDataProjectFolder, ApplicationFileName);
            CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
            NameFolderForConfigs = "Config";
            NameFolderForCache = "EntitiesCache";
            NameMainConfig = "config.json";
            NameCacheFile = "entities_cache.json";
            LimitReceivingEntitiesFromApi = 100;
            ApiMainEndpoint = "api/crm";
            ApiLoginEndpoint = "api/crm_login";
        }
    }
}
