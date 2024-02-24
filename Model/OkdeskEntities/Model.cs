using Newtonsoft.Json;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Model : ViewModelBase
    {
        private string code;
        private string name;

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get { return code; } set { code = value; OnPropertyChanged(nameof(Code)); } }

        [JsonProperty("name")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("visible")]
        public bool? Visible { get; set; }

        [JsonProperty("equipment_kind")]
        public Kind EquipmentKind { get; set; }

        [JsonProperty("equipment_manufacturer")]
        public Manufacturer EquipmentManufacturer { get; set; }
    }
}
