using AqbaApp.Interfaces;
using System;

namespace AqbaApp.Model.OkdeskReport
{
    public class Employee : IEntity, IComparable
    {
        public int Id { get; set; }
        public long SolvedIssues { get; set; }
        public double SpentedTime { get; set; }
        public Issue[] Issues { get; set; } = [];   

        public string LastName { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string Patronymic { get; set; } = string.Empty;
        public long OpenTasks { get; set; }

        public string FullName { get { return $"{LastName} {FirstName} {Patronymic}"; } }
        public bool IsSelected { get; set; } = true;

        public string SpentedTimeString
        {
            get
            {
                var hours = Math.Truncate(SpentedTime);
                var minutes = Math.Round((SpentedTime - hours) * 60);
                if (minutes == 60)
                {
                    hours++;
                    minutes = 0;
                }
                return $"{hours} ч. {minutes} м.";
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj != null && obj is Employee employee)
            {
                return Id == employee.Id;
            }
            return false;
        }

        public int CompareTo(object? obj)
        {
            if (obj != null && obj is Employee o)
            {
                if (SolvedIssues < o.SolvedIssues)
                    return 1;
                else if (SolvedIssues > o.SolvedIssues)
                    return -1;
                else
                    return 0;
            }
            else throw new ArgumentException("Некорректное значение параметра CompareTo");
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}