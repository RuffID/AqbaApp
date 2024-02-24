using Newtonsoft.Json;

namespace AqbaApp.Model.OkdeskEntities
{
    public class KindParameter
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("field_type")]
        public string FieldType { get; set; }
    }
}
