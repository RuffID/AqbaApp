using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskEntities
{
    public class Company : NotifyProperty, IEntity, IHasName
    {
        private int id;
        private string name = string.Empty;
        private string additional_name = string.Empty;
        private bool? active;
        private Category category = new();

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public string AdditionalName { get { return additional_name; } set { additional_name = value; OnPropertyChanged(nameof(AdditionalName)); } }
        public bool? Active { get { return active; } set { active = value; OnPropertyChanged(nameof(Active)); } }
        public Category Category { get { return category; } set { category = value; OnPropertyChanged(nameof(Category)); } }

        public void Replace(Company company)
        {
            Name = company.Name;
            AdditionalName = company.AdditionalName;
            Active = company.Active;
        }
    }
}
