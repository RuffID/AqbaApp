using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using AqbaApp.Model.OkdeskEntities;
using AqbaApp.Model.Authorization;
using static System.Web.HttpUtility;
using AqbaApp.Helper;
using AqbaApp.Model.OkdeskReport;
using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace AqbaApp.API
{
    public static class Request
    {
        
        static readonly SocketsHttpHandler socketsHandler = new()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(2),
        };

        static readonly string serverAddress;

        static Request()
        {
            if (string.IsNullOrEmpty(Config.Settings.ServerAddress))
                serverAddress = "http://127.0.0.1";
            else 
                serverAddress = Config.Settings.ServerAddress;
        }

        public static async Task GetCompanies(ObservableCollection<Company> companies, ObservableCollection<Category> categories)
        {
            companies.Clear();
            int lastCompanyId = 0;
            Company[] comp;

            foreach (var category in categories)
            {
                while (true)
                {
                    var response = await GetResponse($"{serverAddress}/category/companies?categoryId={category.Id}&companyId={lastCompanyId}");

                    if (!string.IsNullOrEmpty(response) && response != "[]")
                    {
                        try
                        {
                            comp = JsonConvert.DeserializeObject<Company[]>(response);

                            if (comp.Length > 0)
                                lastCompanyId = comp[comp.Length - 1].Id + 1;

                            // Если получен не пустой список, то добавляет поочерёдно каждую компанию в список
                            foreach (var client in comp)
                            {
                                companies.Add(client);
                            }
                        }
                        catch (Exception e)
                        {
                            WriteLog.Error($"{e}");
                        }
                    }
                    else
                    {
                        lastCompanyId = 0;
                        break;
                    }
                }
            }
        }

        public static async Task<Company> GetCompany(int companyId)
        {
            var response = await GetResponse($"{serverAddress}/company?companyId={companyId}");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    return JsonConvert.DeserializeObject<Company>(response);
                }
                catch (Exception e)
                {
                    WriteLog.Error($"{e}");
                    return null;
                }
            }
            return null;
        }

        public static async Task<MaintenanceEntity> GetMaintenanceEntity(int maintenanceEntityId)
        {
            var response = await GetResponse($"{serverAddress}/maintenanceEntity?maintenanceEntityId={maintenanceEntityId}");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    return JsonConvert.DeserializeObject<MaintenanceEntity>(response);
                }
                catch (Exception e)
                {
                    WriteLog.Error($"{e}");
                    return null;
                }
            }
            return null;
        }

        public static async Task GetMaintenanceEntities(ObservableCollection<MaintenanceEntity> objects)
        {
            objects.Clear();
            int lastObjectId = 0;
            MaintenanceEntity[] obj;

            while (true)
            {
                var response = await GetResponse($"{serverAddress}/maintenanceEntity/list?maintenanceEntityId={lastObjectId}");

                if (!string.IsNullOrEmpty(response) && response != "[]")
                {
                    try
                    {
                        obj = JsonConvert.DeserializeObject<MaintenanceEntity[]>(response);

                        if (obj.Length > 0)
                            lastObjectId = obj[obj.Length - 1].Id + 1;

                        foreach (var me in obj)
                            objects.Add(me);
                    }
                    catch (Exception e)
                    {
                        WriteLog.Error($"{e}");
                    }
                }
                else break;
            }
        }

        public static async Task<bool> GetCategories(ObservableCollection<Category> categories)
        {
            categories.Clear();
            var response = await GetResponse($"{serverAddress}/category/list");
            Category[] cat;

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    cat = JsonConvert.DeserializeObject<Category[]>(response);

                    if (cat != null || cat.Length >= 0)
                        // Если получен не пустой список, то добавляет поочерёдно каждую компанию в список
                        foreach (var category in cat)
                        {
                            categories.Add(category);
                        }
                    return true;
                }
                catch (Exception e)
                {
                    WriteLog.Error(e.ToString());
                    return false;
                }
            }
            return false;
        }

        public static async Task GetEquipmentsByMaintenanceEntity(ObservableCollection<Equipment> equipments, int objectId)
        {
            equipments.Clear();
            Equipment[] tempEquip;

            var response = await GetResponse($"{serverAddress}/equipment/maintenanceEntity?maintenanceEntityId={objectId}");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    tempEquip = JsonConvert.DeserializeObject<Equipment[]>(response);

                    if (tempEquip != null || tempEquip.Length > 0)
                        foreach (var equip in tempEquip)
                            equipments.Add(equip);
                }
                catch (Exception e)
                {
                    WriteLog.Error($"{e}");
                }
            }
        }

        public static async Task GetEquipmentsByCompany(ObservableCollection<Equipment> equipments, int companyId)
        {
            equipments.Clear();
            Equipment[] tempEquip;

            var response = await GetResponse($"{serverAddress}/equipment/company?companyId={companyId}");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    tempEquip = JsonConvert.DeserializeObject<Equipment[]>(response);

                    if (tempEquip != null || tempEquip.Length > 0)
                        foreach (var equip in tempEquip)
                            equipments.Add(equip);
                }
                catch (Exception e)
                {
                    WriteLog.Error($"{e}");
                }
            }
        }

        public static async Task<List<Employee>> GetEmployees()
        {
            List<Employee> employees = [];
            int lastEmployeeId = 0;
            Employee[] employeeTemp;


            while (true)
            {
                var response = await GetResponse($"{serverAddress}/employee?employeeId={lastEmployeeId}");

                if (!string.IsNullOrEmpty(response) && response != "[]")
                {
                    try
                    {
                        employeeTemp = JsonConvert.DeserializeObject<Employee[]>(response);

                        if (employeeTemp.Length > 0)
                            lastEmployeeId = employeeTemp[employeeTemp.Length - 1].Id + 1;

                        // Если получен не пустой список, то добавляет поочерёдно каждую компанию в список
                        foreach (var emp in employeeTemp)
                        {
                            employees.Add(emp);
                        }
                    }
                    catch (Exception e)
                    {
                        WriteLog.Error($"{e}");
                    }
                }
                else
                {
                    lastEmployeeId = 0;
                    break;
                }
            }

            return employees;

        }

        public static async Task<bool> GetGroups(ObservableCollection<Group> groups)
        {
            groups.Clear();
            Group[] group;

            var response = await GetResponse($"{serverAddress}/group");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    group = JsonConvert.DeserializeObject<Group[]>(response);

                    foreach (var gr in group)
                    {
                        groups.Add(gr);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    WriteLog.Error($"{e}");
                    return false;
                }
            }
            return false;
        }

        public static async Task<bool> GetStatuses(ObservableCollection<Status> statuses)
        {
            statuses.Clear();
            Status[] status;

            var response = await GetResponse($"{serverAddress}/issueStatus");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    status = JsonConvert.DeserializeObject<Status[]>(response);

                    foreach (var st in status)
                    {
                        statuses.Add(st);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    WriteLog.Error(e.ToString());
                    return false;
                }
            }
            return false;
        }

        public static async Task<bool> GetTypes(ObservableCollection<TaskType> types)
        {
            types.Clear();
            TaskType[] type;

            var response = await GetResponse($"{serverAddress}/issueType");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    type = JsonConvert.DeserializeObject<TaskType[]>(response);

                    foreach (var tp in type)
                    {
                        types.Add(tp);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    WriteLog.Error(e.ToString());
                    return false;
                }
            }
            return false;
        }

        public static async Task<bool> GetPriorities(ObservableCollection<Priority> priorities)
        {
            priorities.Clear();
            Priority[] priority;

            var response = await GetResponse($"{serverAddress}/issuePriority");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    priority = JsonConvert.DeserializeObject<Priority[]>(response);

                    foreach (var pt in priority)
                    {
                        priorities.Add(pt);
                    }

                    return true;
                }
                catch (Exception e)
                {
                    WriteLog.Error(e.ToString());
                    return false;
                }
            }
            return false;
        }

        public static async Task GetEmployeePerformance(List<Employee> employees, DateTime dateFrom, DateTime dateTo, string requestType)
        {
            foreach (var emp in employees)
            {
                emp.SolvedTasks = 0;
                emp.SpentedTimeDouble = 0;
            }

            EmployeePerformance[] employeePerformanceList;

            var response = await GetResponse(
                $"{serverAddress}/employeePerformance/list?requestType={requestType}&dateFrom={dateFrom:yyyy.MM.dd}&dateTo={dateTo:yyyy.MM.dd}");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    employeePerformanceList = JsonConvert.DeserializeObject<EmployeePerformance[]>(response);

                    foreach (var emp in employeePerformanceList)
                    {
                        var e = employees.Find(e => e.Id == emp.Id);    // Поиск сотрудника по id из общего списка который был загружен ранее
                        int index = employees.IndexOf(e);   // Получение id сотрудника
                        employees[index].SolvedTasks = emp.SolvedTasks; // Назначает решённые задачи и списанное время по id
                        employees[index].SpentedTimeDouble = emp.SpentedTime;
                        employees[index].Issues = emp.Issues;
                    }
                }
                catch (Exception e)
                {
                    WriteLog.Error(e.ToString());
                }
            }
        }

        public static async Task<Equipment> GetEquipment(int equipment)
        {
            var response = await GetResponse($"{serverAddress}/equipment?equipmentId={equipment}");

            if (!string.IsNullOrEmpty(response) && response != "[]")
            {
                try
                {
                    return JsonConvert.DeserializeObject<Equipment>(response);

                }
                catch (Exception e)
                {
                    WriteLog.Error(e.ToString());
                }
            }
            return null;
        }

        public static async Task<AuthenticateResponse> RefreshRefreshToken(string refreshToken)
        {
            string link = $"{serverAddress}/user/refresh?refreshToken=";
            string linkEncoded = link + UrlEncode(refreshToken).ToString();
            var response = await PutResponse(linkEncoded);

            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    return JsonConvert.DeserializeObject<AuthenticateResponse>(response);
                }
                catch (Exception e) { WriteLog.Warn(e.ToString()); return null; }
            }
            return null;
        }

        public static async Task<AuthenticateResponse> Login(string email, string password)
        {
            string link = $"{serverAddress}/user/login";
            var payload = new AuthenticateRequest(email, password);
            string stringPayload = string.Empty;
            try
            {
                stringPayload = JsonConvert.SerializeObject(payload);
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
            }

            var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var response = await PostResponse(link, content);

            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    return JsonConvert.DeserializeObject<AuthenticateResponse>(response);
                }
                catch (Exception e) { WriteLog.Warn(e.ToString()); return null; }
            }
            return null;
        }

        public static async Task<string> GetLastUpdateTime()
        {
            var response = await GetResponse($"{serverAddress}/employeePerformance/time");

            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var date = Convert.ToDateTime(response.Replace("\"", "").Replace("\\", ""));
                    return $"Время: {date:HH:mm}";
                }
                catch (Exception ex) { WriteLog.Warn(ex.ToString()); return null; }                
            }
            return null;
        }

        public static async Task<string> GetResponse(string link)
        {
            try
            {
                if (!await Auth.CheckAndRefreshToken())
                    return null;

                HttpClient httpClient = new(socketsHandler);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Config.Settings.Token.AccessToken);

                using HttpResponseMessage response = await httpClient.GetAsync(link);
                if (!response.IsSuccessStatusCode)
                    return null;
                else return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
        }

        public static async Task<string> PostResponse(string link, StringContent content)
        {
            try
            {
                HttpClient httpClient = new(socketsHandler);
                var response = await httpClient.PostAsync(link, content);
                if (!response.IsSuccessStatusCode)
                    return null;
                else
                    return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
        }

        public static async Task<string> PutResponse(string link, StringContent content = null)
        {
            try
            {
                HttpClient httpClient = new(socketsHandler);
                var response = await httpClient.PutAsync(link, content);
                if (!response.IsSuccessStatusCode)
                    return null;
                else
                    return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return null;
            }
        }

        public static async Task<Stream> GetFileStream(string fileUrl)
        {
            HttpClient httpClient = new(socketsHandler);
            try
            {
                Stream fileStream = await httpClient.GetStreamAsync(fileUrl);
                return fileStream;
            }
            catch (Exception ex)
            {
                WriteLog.Warn(ex.ToString());
                return Stream.Null;
            }
        }
    }
}