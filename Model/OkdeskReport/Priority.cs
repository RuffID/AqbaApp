using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskReport
{
    public class Priority : NotifyProperty, IOkdeskDictionary
    {
        private long id;
        private string name = string.Empty;
        private bool isChecked = true;        

        public long Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
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
