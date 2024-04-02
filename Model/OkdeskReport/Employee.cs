using System;

namespace AqbaApp.Model.OkdeskReport
{
    public class Employee : IComparable
    {
        public int Id { get; set; }
        public string Last_name { get; set; }
        public string First_name { get; set; }
        public string Patronymic { get; set; }
        public string Position { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Phone { get; set; }
        public string Comment { get; set; }
        public Group[] Groups { get; set; }
        public Role[] Roles { get; set; }
        public string FullName { get { return $"{Last_name} {First_name} {Patronymic}"; } }
        public Issue[] Issues { get; set; }
        public int SolvedTasks { get; set; }
        public int OpenTasks { get; set; }
        public double SpentedTimeDouble { get; set; }
        public string SpentedTime
        {
            get
            {
                var hours = Math.Truncate(SpentedTimeDouble);
                var minutes = Math.Round((SpentedTimeDouble - hours) * 60);
                return $"{hours} ч. {minutes} м.";
            }
        }        
        public bool IsSelected { get; set; } = true;

        public Employee() { }
        public Employee(Employee employee)
        {
            Id = employee.Id;
            Last_name = employee.Last_name;
            First_name = employee.First_name;
            Patronymic = employee.Patronymic;
            Position = employee.Position;
            Email = employee.Email;
            Login = employee.Login;
            Phone = employee.Phone;
            Comment = employee.Comment;
            Groups = employee.Groups;
            Roles = employee.Roles;
            OpenTasks = employee.OpenTasks;
            SolvedTasks = employee.SolvedTasks;
            Issues = employee.Issues;
            IsSelected = employee.IsSelected;
        }

        public override bool Equals(object obj)
        {
            if (obj is Employee employee)
            {
                return Id == employee.Id;
            }
            return false;
        }

        public int CompareTo(object obj)
        {
            if (obj is Employee o)
            {
                if (SolvedTasks < o.SolvedTasks)
                    return 1;
                else if (this.SolvedTasks > o.SolvedTasks)
                    return -1;
                else
                    return 0;
            }
            else throw new ArgumentException("Некорректное значение параметра CompareTo");
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
