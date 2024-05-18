using AqbaApp.Interfaces;

namespace AqbaApp.Model.OkdeskReport
{
    public class TaskType : ViewModelBase, IOkdeskDictionary
    {
        public TaskType() { }

        public TaskType(IOkdeskDictionary type)
        {
            id = type.Id;
            name = type.Name;
            IsChecked = type.IsChecked;
        }

        private int id;
        private string name;
        private bool isChecked = true;

        public int Id { get { return id; } set { id = value; OnPropertyChanged(nameof(Id)); } }
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
