using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskEntities
{
    public class MaintenanceEntity : NotifyProperty, IEntity, IHasName
    {
        private int id;
        private int companyId;
        private string name = string.Empty;
        private string address = string.Empty;

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string Address { get { return address; } set { address = value; OnPropertyChanged(nameof(Address)); } }
        public bool Active { get; set; }
        public int CompanyId { get { return companyId; } set { companyId = value; OnPropertyChanged(nameof(CompanyId)); } }

        public void Replace(MaintenanceEntity entity)
        {
            Name = entity.Name;
            Address = entity.Address;
            Active = entity.Active;
            CompanyId = entity.CompanyId;
        }
    }
}
