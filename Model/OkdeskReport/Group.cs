using System.Collections.Generic;

namespace AqbaApp.Model.OkdeskReport
{
    public class Group : ViewModelBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public int[] EmployeesId { get; set; }
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

        public Group() 
        {
            Employees = new List<Employee>();
            EmployeesId = [];
        }
    }
}
