namespace AqbaApp.Model.OkdeskReport
{
    public class Priority : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Position { get; set; }
        public bool Default { get; set; }
        public string Color { get; set; }

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
    }
}
