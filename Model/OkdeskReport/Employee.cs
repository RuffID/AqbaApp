using AqbaApp.Interfaces;
using Newtonsoft.Json;
using System;

namespace AqbaApp.Model.OkdeskReport
{
    public class Employee : IComparable, IEntity
    {
        public int Id { get; set; }
        public int SolvedIssues { get; set; }
        public double SpentedTime { get; set; }
        public Issue[] Issues { get; set; }
        public string Last_name { get; set; }
        public string First_name { get; set; }
        public string Patronymic { get; set; }

        [JsonIgnore]
        public int OpenTasks { get; set; }
        [JsonIgnore]
        public string FullName { get { return $"{Last_name} {First_name} {Patronymic}"; } }
        [JsonIgnore]
        public bool IsSelected { get; set; } = true;
        [JsonIgnore]
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