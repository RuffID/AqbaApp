using Newtonsoft.Json;

namespace AqbaApp.Model.OkdeskEntities
{
    public class EquipmentParameter : ViewModelBase
    {
        private string code;
        private string name;
        private string fieldType;
        private string value;

        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty("code")]
        public string Code { get { return code; } set { code = value; OnPropertyChanged(nameof(Code)); } }        

        [JsonProperty("name")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }

        [JsonProperty("field_type")]
        public string FieldType { get { return fieldType; } set { fieldType = value; OnPropertyChanged(nameof(FieldType)); } }

        [JsonProperty("value")]
        public string Value { get { return value; } set { this.value = value; OnPropertyChanged(nameof(Value)); } }

        /*public KindParameter KindParam { get; set; }
        public Equipment Equipment { get; set; }*/
    }
}
