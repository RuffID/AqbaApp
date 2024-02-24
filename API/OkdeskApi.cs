/*using AqbaApp.Model.OkdeskReportTask;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace AqbaApp.API
{
    public static class OkdeskApi
    {
        static readonly string defaultLink = $"{Immutable.okdeskApiLink}/issues/count?api_token={Immutable.apiToken}";
        static readonly SocketsHttpHandler socketsHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };
        static readonly HttpClient httpClient = new(socketsHandler);

        

        public static async Task GetReportSolvedTasks(ICollection<Employee> employees, DateTime dateFrom, DateTime dateTo)    // Получение списка открытых заявок
        {
            StringBuilder linkSolvedTasks = new();
            int[] numberOfSolvedTask = null;

            foreach (var employee in employees)
            {
                linkSolvedTasks.Clear();
                linkSolvedTasks.Append(defaultLink + $"&assignee_ids[]={employee.Id}&completed_since={dateFrom:dd-MM-yyyy} 00:00&completed_until={dateTo:dd-MM-yyyy} 23:59");
                var responseSolved = await GetResponse(linkSolvedTasks.ToString());
                if (responseSolved != null && responseSolved != "[]")
                {
                    try
                    {
                        numberOfSolvedTask = JsonConvert.DeserializeObject<int[]>(responseSolved);
                    }
                    catch (Exception e) { WriteLog.Error(e.ToString()); }

                    if (numberOfSolvedTask?.Length == 0 || numberOfSolvedTask?.Length == null)
                        employee.SolvedTasks = 0;
                    else
                    {
                        employee.SolvedTasks = numberOfSolvedTask.Length;
                        employee.IsSelected = true;
                    }

                    numberOfSolvedTask = null;
                }
                else
                    employee.SolvedTasks = 0;
            }
        }

        public static async Task GetReportOpenTasks(ICollection<Employee> employees, ICollection<Status> statuses, ICollection<TaskType> types)    // Получение списка открытых заявок
        {
            StringBuilder linkOpenTasks = new();
            int[] numberOfTasks = null;

            foreach (var employee in employees)
            {
                linkOpenTasks.Clear();
                linkOpenTasks.Append(defaultLink);

                foreach (var status in statuses)
                    if (status.IsChecked == true)
                        linkOpenTasks.Append($"&status[]={status.Code}");

                foreach (var type in types)
                    if (type.IsChecked == true)
                        linkOpenTasks.Append($"&type[]={type.Code}");

                linkOpenTasks.Append($"&assignee_ids[]={employee.Id}");

                var responseOpen = await GetResponse(linkOpenTasks.ToString());

                if (responseOpen != null && responseOpen != "[]")
                {
                    try
                    {
                        numberOfTasks = JsonConvert.DeserializeObject<int[]>(responseOpen);
                    }
                    catch (Exception e) { WriteLog.Error(e.ToString()); }

                    if (numberOfTasks?.Length == 0 || numberOfTasks?.Length == null)
                        employee.OpenTasks = 0;
                    else
                    {
                        employee.OpenTasks = numberOfTasks.Length;
                        employee.IsSelected = true;
                    }

                    numberOfTasks = null;
                }
                else
                    employee.OpenTasks = 0;
            }
        }

        

        public static async Task GetTypes(ICollection<TaskType> types)
        {
            types?.Clear();
            string link = Immutable.okdeskApiLink + "/dictionaries/issues/types?api_token=" + Immutable.apiToken;
            TaskType[] tempTypes = Array.Empty<TaskType>();
            var response = await GetResponse(link);

            if (response != null && response != "[]")
            {
                try
                {
                    tempTypes = JsonConvert.DeserializeObject<TaskType[]>(response);
                }
                catch (Exception e) { WriteLog.Error(e.ToString()); }

                if (tempTypes != null && tempTypes.Length > 0)
                    foreach (var type in tempTypes)
                        types?.Add(type);
            }

            // Цикл вытаскивает все типы из "папок" в общий список для комфортного отображения
            if (types?.Count > 0)
            {
                foreach (var type in types.ToList())
                {
                    if (type?.Children?.Length > 0)
                    {
                        foreach (var child in type.Children)
                        {
                            if (child != null)
                            {
                                types.Add(new(child));
                            }
                        }
                        types.Remove(type);
                    }
                }
            }
        }

        public static async Task GetStatuses(ICollection<Status> statuses)
        {
            statuses?.Clear();
            string link = Immutable.okdeskApiLink + "/issues/statuses/?api_token=" + Immutable.apiToken;
            Status[] tempSt = Array.Empty<Status>();
            var response = await GetResponse(link);

            if (response != null && response != "[]")
            {
                try
                {
                    tempSt = JsonConvert.DeserializeObject<Status[]>(response);
                }
                catch (Exception e) { WriteLog.Error(e.ToString()); }

                if (tempSt != null && tempSt.Length > 0)
                    foreach (var status in tempSt)
                        statuses?.Add(status);
            }
        }

        
    }
}*/
