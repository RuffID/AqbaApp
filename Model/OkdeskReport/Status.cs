namespace AqbaApp.Model.OkdeskReport
{
    public class Status : ViewModelBase
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        private bool isChecked = false;
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
