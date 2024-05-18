using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskReport
{
    public class Group : ViewModelBase, IOkdeskDictionary
    {
        public Group() { }

        public Group(IOkdeskDictionary group)
        {
            id = group.Id;
            name = group.Name;
            isChecked = group.IsChecked;
        }

        private int id;
        private string name;
        private bool isChecked = true;

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }
        public bool IsChecked { get { return isChecked; } set { isChecked = value; OnPropertyChanged(nameof(IsChecked)); } }

        public void Update(IOkdeskDictionary group)
        {
            Id = group.Id;
            Name = group.Name;
            IsChecked = group.IsChecked;
        }

        public void UpdateWithoutChecked(IOkdeskDictionary group)
        {
            Id = group.Id;
            Name = group.Name;
        }
    }
}
