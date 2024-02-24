using Newtonsoft.Json;
using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Kind : ViewModelBase
    {
        private string code;
        private string name;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get { return code;} set { code = value; OnPropertyChanged(nameof(Code)); } }

        [JsonProperty("name")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("visible")]
        public bool? Visible { get; set; }

        [JsonProperty("parameters")]
        public ICollection<KindParameter> Parameters { get; set; }
    }
}
