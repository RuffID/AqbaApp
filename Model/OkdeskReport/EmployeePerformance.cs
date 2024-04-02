namespace AqbaApp.Model.OkdeskReport
{
    public class EmployeePerformance
    {
        public int Id { get; set; }
        public int SolvedTasks { get; set; }
        public double SpentedTime { get; set; }
        public Issue[] Issues { get; set; }

    }
}
