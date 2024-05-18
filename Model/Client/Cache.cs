using AqbaApp.Interfaces;
using AqbaApp.Model.OkdeskReport;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AqbaApp.Model.Client
{
    public class Cache : IConfig
    {
        [JsonProperty]
        public ICollection<Group> Groups; /*{ get; set; }*/
        [JsonProperty]
        public ICollection<Status> Statuses; /*{ get; set; }*/
        [JsonProperty]
        public ICollection<TaskType> Types; /*{ get; set; }*/
        [JsonProperty]
        public ICollection<Priority> Priorities; /*{ get; set; }*/


        [JsonIgnore]
        public Formatting Formatting {  get; set; }
        [JsonIgnore]
        public string PathToFile { get; set; }
        [JsonIgnore]
        public string PathToFolder { get; set; }

        public Cache()
        {
            Groups = [];
            Statuses = [];
            Types = [];
            Priorities = [];
            Formatting = Formatting.None;
            PathToFolder = Immutable.cacheFolderName;
            PathToFile = Immutable.cachePath;
        }
    }
}
