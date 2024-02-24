namespace AqbaApp.Model.OkdeskReport
{
    public class TaskType : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Default { get; set; }
        public bool Inner { get; set; }
        public bool Available_for_client { get; set; }
        public string Type { get; set; }
        public TaskType[] Children { get; set; }
        private bool isChecked = true;
        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public TaskType() { }

        public TaskType(TaskType taskType)
        {
            Id = taskType.Id;
            Name = taskType.Name;
            Code = taskType.Code;
            Default = taskType.Default;
            Inner = taskType.Inner;
            Available_for_client = taskType.Available_for_client;
            Type = taskType.Type;
            Children = taskType.Children;
            IsChecked = taskType.IsChecked;
        }
    }
}
