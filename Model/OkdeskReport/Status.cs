using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskReport
{
    public class Status : ViewModelBase, IOkdeskDictionary
    {
        public Status() { }

        public Status(IOkdeskDictionary status)
        {
            id = status.Id;
            isChecked = status.IsChecked;
            name = status.Name;
        }

        private int id;
        private string name;
        private bool isChecked = true;

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }        
        public bool IsChecked { get { return isChecked; } set { isChecked = value; OnPropertyChanged(nameof(IsChecked)); } }

        public void Update(IOkdeskDictionary status)
        {
            Id = status.Id;
            Name = status.Name;
            IsChecked = status.IsChecked;
        }

        public void UpdateWithoutChecked(IOkdeskDictionary status)
        {
            Id = status.Id;
            Name = status.Name;
        }
    }
}
