using Newtonsoft.Json;
using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Equipment : ViewModelBase
    {
        private List<EquipmentParameter> parameters;
        private string serial_number;
        private string inventory_number;
        private string fullName;
        private Kind kind;
        private Manufacturer manufacturer;
        private Model model;
        

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("serial_number")]
        public string Serial_number { get { return serial_number; } set { serial_number = value; OnPropertyChanged(nameof(Serial_number)); } }

        [JsonProperty("inventory_number")]
        public string Inventory_number { get { return inventory_number; } set { inventory_number = value; OnPropertyChanged(nameof(Inventory_number)); } }

        [JsonProperty("parameters")]
        public List<EquipmentParameter> Parameters { get { return parameters; } set { parameters = value; OnPropertyChanged(nameof(Parameters)); } }

        [JsonProperty("equipment_kind")]
        public Kind Equipment_kind { get { return kind; } set { kind = value; OnPropertyChanged(nameof(Equipment_kind)); } }

        [JsonProperty("equipment_manufacturer")]
        public Manufacturer Equipment_manufacturer { get { return manufacturer; } set { manufacturer = value; OnPropertyChanged(nameof(Equipment_manufacturer)); } }

        [JsonProperty("equipment_model")]
        public Model Equipment_model { get { return model; } set { model = value; OnPropertyChanged(nameof(Equipment_model)); } }

        [JsonProperty("company")]
        public Company Company { get; set; }

        [JsonProperty("maintenance_entity")]
        public MaintenanceEntity Maintenance_entity { get; set; }

        public string FullName
        {
            get
            {
                fullName =               
                    $"{Equipment_kind?.Name} " +
                    $"{Parameters?.Find(p => p.Code == "0001")?.Value} " + // Роль
                    $"{Parameters?.Find(p => p.Code == "0019")?.Value} " + // Название терминала
                    $"{Inventory_number} " +
                    $"{Equipment_model?.Name}";
                return fullName;
            }
            set { fullName = value; OnPropertyChanged(nameof(FullName)); }

        }

        public void Replace(Equipment newEquipment)
        {
            Id = newEquipment.Id;
            Serial_number = newEquipment.Serial_number;
            Inventory_number = newEquipment.Inventory_number;
            Parameters = newEquipment.Parameters;
            Equipment_kind = newEquipment.Equipment_kind;
            Equipment_manufacturer = newEquipment.Equipment_manufacturer;
            Equipment_model = newEquipment.Equipment_model;
            Company = newEquipment.Company;
            Maintenance_entity = newEquipment.Maintenance_entity;
            FullName = newEquipment.FullName;
        }
    }
}
