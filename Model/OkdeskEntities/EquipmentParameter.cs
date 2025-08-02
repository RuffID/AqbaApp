using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskEntities
{
    public class EquipmentParameter : NotifyProperty, IEntity
    {
        private string code = string.Empty;
        private string name = string.Empty;
        private string value = string.Empty;

        public int Id { get; set; }
        public string Code { get { return code; } set { code = value; OnPropertyChanged(nameof(Code)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string Value { get { return value; } set { this.value = value; OnPropertyChanged(nameof(Value)); } }
    }
}
