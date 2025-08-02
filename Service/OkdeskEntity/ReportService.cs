using System.Threading.Tasks;
using System.Collections.Generic;
using AqbaApp.Core.Api;
using AqbaApp.Model.Client;
using AqbaApp.Model.OkdeskReport;
using System;
using System.Linq;
using AqbaApp.Service.Client;
namespace AqbaApp.Service.OkdeskEntity
{
    public class ReportService(SettingService<MainSettings> mainSettings, Immutable immutable, GetItemService request)
    {
        private async Task<List<ReportInfo>?> GetReportFromCloudApi(DateTime dateFrom, DateTime dateTo)
        {
            dateTo = new(dateTo.Year, dateTo.Month, dateTo.Day, hour: 23, minute: 59, second: 59);
            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/report?dateFrom={dateFrom:yyyy-MM-dd}&dateTo={dateTo:yyyy-MM-dd HH:mm:ss}";

            return await request.GetRangeOfItems<ReportInfo>(link);
        }

        public async Task<bool> GetEmployeePerformance(List<Employee> employees, DateTime dateFrom, DateTime dateTo)
        {
            foreach (var emp in employees)
            {
                emp.SolvedIssues = 0;
                emp.SpentedTime = 0;
                emp.Issues = [];
            }

            List<ReportInfo>? reportList = await GetReportFromCloudApi(dateFrom, dateTo);

            if (reportList == null || reportList.Count == 0)
                return false;

            foreach (var employeeFromReport in reportList)
            {
                Employee? employee = employees.FirstOrDefault(e => e.Id == employeeFromReport.EmployeeId);    // Поиск сотрудника по id из общего списка который был загружен ранее
                if (employee == null) continue;

                int index = employees.IndexOf(employee);   // Получение id сотрудника
                if (index == -1) continue;

                if (employeeFromReport?.SolvedIssues != null)
                    employees[index].SolvedIssues = employeeFromReport.SolvedIssues ?? 0; // Назначает решённые задачи и списанное время по id

                if (employeeFromReport?.SpentedTime != null)
                    employees[index].SpentedTime = employeeFromReport.SpentedTime ?? 0;

                if (employeeFromReport?.Issues != null)
                    employees[index].Issues = employeeFromReport.Issues;
            }

            return true;
        }
    }
}
