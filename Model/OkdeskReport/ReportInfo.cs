﻿namespace AqbaApp.Model.OkdeskReport
{
    public class ReportInfo
    {
        public long EmployeeId { get; set; }
        public long? SolvedIssues { get; set; }
        public double? SpentedTime { get; set; }
        public Issue[]? Issues { get; set; }
    }
}