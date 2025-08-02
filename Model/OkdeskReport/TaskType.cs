using AqbaApp.Core;
using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskReport
{
    public class TaskType : NotifyProperty, IOkdeskDictionary
    {
        private long id;
        private string name = string.Empty;
        private bool isChecked = true;

        public long Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
        public string Name { get { return name; } set { name = value; OnPropertyChanged(nameof(Name)); } }        
        public bool IsChecked { get { return isChecked; } set { isChecked = value; OnPropertyChanged(nameof(IsChecked)); } }

        public void Update(IOkdeskDictionary taskType)
        {
            Id = taskType.Id;
            Name = taskType.Name;
            IsChecked = taskType.IsChecked;
        }

        public void UpdateWithoutChecked(IOkdeskDictionary taskType)
        {
            Id = taskType.Id;
            Name = taskType.Name;
        }
    }
}
