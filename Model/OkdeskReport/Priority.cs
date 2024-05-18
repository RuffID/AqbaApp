using AqbaApp.Dto;
using AqbaApp.Interfaces;
using System.Text.Json.Serialization;

namespace AqbaApp.Model.OkdeskReport
{
    public class Priority : ViewModelBase, IOkdeskDictionary
    {
        public Priority() { }
        
        public Priority(IOkdeskDictionary priority)
        {
            id = priority.Id;
            name = priority.Name;
            isChecked = priority.IsChecked;
        }

        private int id;
        private string name;
        private bool isChecked = true;        

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public bool IsChecked { get { return isChecked; } set { isChecked = value; OnPropertyChanged(nameof(IsChecked)); } }

        public void Update(IOkdeskDictionary priority)
        {
            Id = priority.Id;
            Name = priority.Name;
            IsChecked = priority.IsChecked;
        }

        public void UpdateWithoutChecked(IOkdeskDictionary priority)
        {
            Id = priority.Id;
            Name = priority.Name;
        }
    }
}
