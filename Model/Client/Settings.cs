﻿using AqbaApp.Interfaces;
using AqbaApp.Model.Authorization;
using Newtonsoft.Json;

namespace AqbaApp.Model.Client
{
    public class Settings : IConfig
    {
        [JsonProperty]
        public string ServerAddress { get; set; }
        [JsonProperty]
        public string PathToCLEARbat { get; set; }
        [JsonProperty]
        public string PathToAnydesk { get; set; }
        [JsonProperty]
        public string PathToAmmyAdmin { get; set; }
        [JsonProperty]
        public string PathToAssistant { get; set; }
        [JsonProperty]
        public string PathToRustDesk { get; set; }
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
        public int OpenTasksFontSize { get; set; }
        [JsonProperty]
        public string HeaderBackgroundColor { get; set; }
        [JsonProperty]
        public string FullNameBackgroundColor { get; set; }
        [JsonProperty]
        public string SpendedTimeBackgroundColor { get; set; }
        [JsonProperty]
        public string SolvedTasksBackgroundColor { get; set; }
        [JsonProperty]
        public string OpenTasksBackgroundColor { get; set; }        
        [JsonProperty]
        public bool ElectronicQueueMode { get; set; }
        [JsonProperty]
        public bool HideEmployeesWithoutSolvedIssues { get; set; }
        [JsonProperty]
        public bool HideEmployeesWithoutWrittenOffTime { get; set; }
        [JsonProperty]
        public bool HideEmployeesWithoutOpenIssues { get; set; }
        [JsonProperty]
        public AuthenticateResponse Token { get; set; }

        [JsonIgnore]
        public Formatting Formatting { get; set; }
        [JsonIgnore]
        public string PathToFile { get; set; }
        [JsonIgnore]
        public string PathToFolder { get; set; }

        public Settings()
        {
            Token = new AuthenticateResponse();
            Formatting = Formatting.Indented;
            PathToFolder = Immutable.configFolderName;
            PathToFile = Immutable.configPath;
        }
    }
}
