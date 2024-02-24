using Newtonsoft.Json;

namespace AqbaApp.Model.Server
{
    public class AppInfo
    {
        [JsonProperty]
        public static string Version { get; set; }

        [JsonProperty]
        public static string Description { get; set; }
    }
}
