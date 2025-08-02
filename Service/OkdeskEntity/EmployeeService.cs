using System.Threading.Tasks;
using System.Collections.Generic;
using AqbaApp.Core.Api;
using AqbaApp.Model.Client;
using AqbaApp.Model.OkdeskReport;
using AqbaApp.Service.Client;

namespace AqbaApp.Service.OkdeskEntity
{
    public class EmployeeService(SettingService<MainSettings> mainSettings, GetItemService request, Immutable immutable)
    {
        public async Task<List<Employee>?> GetEmployeesFromCloudApi(long startIndex = 0, long limit = 50)
        {
            List<Employee>? employees = [];

            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/employee/list";
            await foreach (List<Employee> employeesFromCloudApi in request.GetAllItems<Employee>(link, startIndex, limit))
            {
                employees.AddRange(employeesFromCloudApi);
            }

            return employees;
        }

        public async Task<List<EmployeeGroup>?> GetEmployeeConnectionsWithGroupsFromCloudApi(SettingService<MainSettings> mainSettings, long startIndex = 0)
        {
            string link = $"{mainSettings.Settings.ServerAddress}/{immutable.ApiMainEndpoint}/employee/connections_with_group";
            return await request.GetRangeOfItems<EmployeeGroup>(link, startIndex);
        }
    }
}
