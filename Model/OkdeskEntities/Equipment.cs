using AqbaApp.Core;
using AqbaApp.Interfaces;
using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Equipment : NotifyProperty, IEntity
    {
        private string serial_number = string.Empty;
        private string inventory_number = string.Empty;
        private string fullName = string.Empty;
        private List<EquipmentParameter> parameters = [];
        private Kind kind = new();
        private Manufacturer manufacturer = new();
        private Model model = new();

        public int Id { get; set; }        
        public string Serial_number { get { return serial_number; } set { serial_number = value; OnPropertyChanged(nameof(Serial_number)); } }        
        public string Inventory_number { get { return inventory_number; } set { inventory_number = value; OnPropertyChanged(nameof(Inventory_number)); } }        
        public List<EquipmentParameter> Parameters { get { return parameters; } set { parameters = value; OnPropertyChanged(nameof(Parameters)); } }        
        public Kind Kind { get { return kind; } set { kind = value; OnPropertyChanged(nameof(Kind)); } }        
        public Manufacturer Manufacturer { get { return manufacturer; } set { manufacturer = value; OnPropertyChanged(nameof(Manufacturer)); } }       
        public Model Model { get { return model; } set { model = value; OnPropertyChanged(nameof(Model)); } }        
        public Company? Company { get; set; }        
        public MaintenanceEntity? Maintenance_entity { get; set; }

        public string FullName
        {
            get
            {
                fullName =               
                    $"{Kind?.Name} " +
                    $"{Parameters?.Find(p => p.Code == "0001")?.Value} " + // Роль
                    $"{Parameters?.Find(p => p.Code == "0019")?.Value} " + // Название терминала
                    $"{Inventory_number} " +
                    $"{Model?.Name}";
                return fullName;
            }
            set { fullName = value; OnPropertyChanged(nameof(FullName)); }
        }

        public void Replace(Equipment equipment)
        {
            Serial_number = equipment.Serial_number;
            Inventory_number = equipment.Inventory_number;
            Parameters = equipment.Parameters;
            Kind = equipment.Kind;
            Manufacturer = equipment.Manufacturer;
            Model = equipment.Model;
        }
    }
}
