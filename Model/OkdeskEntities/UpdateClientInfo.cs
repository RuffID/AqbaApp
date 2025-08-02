using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskEntities
{
    public class UpdateClientInfo
    {
        public Company? Company { get; set; }
        public ICollection<MaintenanceEntity>? MaintenanceEntity { get; set; }
        public ICollection<Equipment>? Equipments { get; set; }
    }
}
