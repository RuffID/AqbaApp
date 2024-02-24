using Newtonsoft.Json;
using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskEntities
{
    public class MaintenanceEntity
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("coordinates")]
        public List<decimal> Coordinates { get; set; }

        [JsonProperty("company_id")]
        public int Company_Id { get; set; }
    }
}
