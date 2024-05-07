namespace AqbaApp.Model.OkdeskReport
{
    public class Status : ViewModelBase
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
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
