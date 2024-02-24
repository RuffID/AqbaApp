using AqbaApp.Model.Authorization;
using Newtonsoft.Json;

namespace AqbaApp.Model.Server
{
    public class Settings
    {
        [JsonProperty]
        public string ServerAddress { get; set; }
        [JsonProperty]
        public string PathToBackground { get; set; }
        [JsonProperty]
        public int HeaderFontSize { get; set; }
        [JsonProperty]
        public int FullNameFontSize { get; set; }
        [JsonProperty]
        public int SolvedTasksFontSize { get; set; }
        [JsonProperty]
        public int SpendedTimeFontSize { get; set; }
        [JsonProperty]
        public string HeaderBackgroundColor { get; set; }
        [JsonProperty]
        public string FullNameBackgroundColor { get; set; }
        [JsonProperty]
        public string SpendedTimeBackgroundColor { get; set; }
        [JsonProperty]
        public string SolvedTasksBackgroundColor { get; set; }
        [JsonProperty]
        public string[] CheckedGroups { get; set; }
        [JsonProperty]
        public bool ElectronicQueueMode { get; set; }
        [JsonProperty]
        public AuthenticateResponse Token { get; set; }

        public Settings()
        {
            Token = new AuthenticateResponse();
        }
    }
}
