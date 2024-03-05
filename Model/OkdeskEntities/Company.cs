using Newtonsoft.Json;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Company : ViewModelBase
    {
        int id;
        string name;
        string additional_name;
        bool? active;
        Category category;

        [JsonProperty("id")]
        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }

        [JsonProperty("name")]
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }

        [JsonProperty("additional_name")]
        public string AdditionalName { get { return additional_name; } set { additional_name = value; OnPropertyChanged(nameof(AdditionalName)); } }

        [JsonProperty("active")]
        public bool? Active { get { return active; } set { active = value; OnPropertyChanged(nameof(Active)); } }

        public Category Category { get { return category; } set { category = value; OnPropertyChanged(nameof(Category)); } }

        public Company()
        {
            Category = new Category();
        }

        public void Replace(Company newCompany)
        {
            Id = newCompany.Id;
            Name = newCompany.Name;
            AdditionalName = newCompany.AdditionalName;
            Active = newCompany.Active;
            Category = newCompany.Category;
        }
    }
}
