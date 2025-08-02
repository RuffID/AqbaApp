using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskReport
{
    public class Group : NotifyProperty, IOkdeskDictionary
    {
        private long id;
        private string name = string.Empty;
        private bool isChecked = true;

        public long Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get => name; set { name = value; OnPropertyChanged(nameof(Name)); } }
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
