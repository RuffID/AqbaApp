using Newtonsoft.Json;
using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskEntities
{
    public class MaintenanceEntity : ViewModelBase
    {
        int id;
        string name;
        string address;
        List<decimal> coords;
        int companyId;

        [JsonProperty("id")]
        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }

        [JsonProperty("name")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }

        [JsonProperty("address")]
        public string Address { get { return address; } set { address = value; OnPropertyChanged(nameof(Address)); } }

        [JsonProperty("coordinates")]
        public List<decimal> Coordinates { get { return coords; } set { coords = value; OnPropertyChanged(nameof(Coordinates)); } }

        [JsonProperty("company_id")]
        public int Company_Id { get { return companyId; } set { companyId = value; OnPropertyChanged(nameof(Company_Id)); } }

        public void Replace(MaintenanceEntity newEntity)
        {
            Id = newEntity.Id;
            Name = newEntity.Name;
            Address = newEntity.Address;
            Coordinates = newEntity.Coordinates;
            Company_Id = newEntity.Company_Id;
        }
    }
}
