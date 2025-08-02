using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Model : NotifyProperty, IEntity
    {
        private string code = string.Empty;
        private string name = string.Empty;

        public int Id { get; set; }
        public string Code { get { return code; } set { code = value; OnPropertyChanged(nameof(Code)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string Description { get; set; } = string.Empty;
        public bool? Visible { get; set; }
        public Kind EquipmentKind { get; set; } = new();
        public Manufacturer EquipmentManufacturer { get; set; } = new();
    }
}
