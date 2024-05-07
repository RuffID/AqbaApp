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
using Notifications.Wpf.Core;

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
                    var response = await GetResponse($"{serverAddress}/category/companies?categoryCode={category.Code}&companyId={lastCompanyId}");

                    if (response.RequestSuccessful == false)
                        _ = Notice.Show(NotificationType.Warning, "Не удалось получить список компаний.\nПроверьте связь с сервером.");


                    if (string.IsNullOrEmpty(response.Response) || response.Response == "[]")
                    {
                        lastCompanyId = 0;
                        break;
                    }

                    try
                    {
                        comp = JsonConvert.DeserializeObject<Company[]>(response.Response);

                        if (comp.Length > 0)
                            lastCompanyId = comp[comp.Length - 1].Id + 1;

                        // Если получен не пустой список, то добавляет поочерёдно каждую компанию в список
                        foreach (var client in comp)
                            companies.Add(client);
                    }
                    catch (Exception e) { WriteLog.Error($"{e}"); }
                }
            }
        }

        public static async Task<Company> GetCompany(int companyId)
        {
            var response = await GetResponse($"{serverAddress}/company?companyId={companyId}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить данные по компании.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return null;

            try
            {
                return JsonConvert.DeserializeObject<Company>(response.Response);
            }
            catch (Exception e) { WriteLog.Error($"{e}"); return null; }
        }

        public static async Task<MaintenanceEntity> GetMaintenanceEntity(int maintenanceEntityId)
        {
            var response = await GetResponse($"{serverAddress}/maintenanceEntity?maintenanceEntityId={maintenanceEntityId}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить данные по объекту.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return null;
            try
            {
                return JsonConvert.DeserializeObject<MaintenanceEntity>(response.Response);
            }
            catch (Exception e) { WriteLog.Error($"{e}"); return null; }
        }

        public static async Task GetMaintenanceEntities(ObservableCollection<MaintenanceEntity> objects)
        {
            objects.Clear();
            int lastObjectId = 0;
            MaintenanceEntity[] obj;

            while (true)
            {
                var response = await GetResponse($"{serverAddress}/maintenanceEntity/list?maintenanceEntityId={lastObjectId}");

                if (response.RequestSuccessful == false)
                    _ = Notice.Show(NotificationType.Warning, "Не удалось получить список объектов.\nПроверьте связь с сервером.");

                if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") break;

                try
                {
                    obj = JsonConvert.DeserializeObject<MaintenanceEntity[]>(response.Response);

                    if (obj.Length > 0)
                        lastObjectId = obj[obj.Length - 1].Id + 1;

                    foreach (var me in obj)
                        objects.Add(me);
                }
                catch (Exception e) { WriteLog.Error($"{e}"); }
            }
        }

        public static async Task<bool> GetCategories(ObservableCollection<Category> categories)
        {
            categories.Clear();
            var response = await GetResponse($"{serverAddress}/category/list");
            Category[] cat;

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список категорий компаний.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return false;
            try
            {
                cat = JsonConvert.DeserializeObject<Category[]>(response.Response);

                if (cat != null || cat.Length >= 0)
                    // Если получен не пустой список, то добавляет поочерёдно каждую компанию в список
                    foreach (var category in cat)
                        categories.Add(category);

                return true;
            }
            catch (Exception e) { WriteLog.Error(e.ToString()); return false; }
        }

        public static async Task GetEquipmentsByMaintenanceEntity(ObservableCollection<Equipment> equipments, int objectId)
        {
            equipments.Clear();
            Equipment[] tempEquip;

            var response = await GetResponse($"{serverAddress}/equipment/maintenanceEntity?maintenanceEntityId={objectId}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список оборудования.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return;
            try
            {
                tempEquip = JsonConvert.DeserializeObject<Equipment[]>(response.Response);

                if (tempEquip != null || tempEquip.Length > 0)
                    foreach (var equip in tempEquip)
                        equipments.Add(equip);
            }
            catch (Exception e) { WriteLog.Error($"{e}"); }
        }

        public static async Task GetEquipmentsByCompany(ObservableCollection<Equipment> equipments, int companyId)
        {
            equipments.Clear();
            Equipment[] tempEquip;

            var response = await GetResponse($"{serverAddress}/equipment/company?companyId={companyId}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список оборудования.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return;
            try
            {
                tempEquip = JsonConvert.DeserializeObject<Equipment[]>(response.Response);

                if (tempEquip != null || tempEquip.Length > 0)
                    foreach (var equip in tempEquip)
                        equipments.Add(equip);
            }
            catch (Exception e) { WriteLog.Error($"{e}"); }
        }

        public static async Task<List<Employee>> GetEmployees()
        {
            List<Employee> employees = [];
            int lastEmployeeId = 0;
            Employee[] employeeTemp;


            while (true)
            {
                var response = await GetResponse($"{serverAddress}/employee?employeeId={lastEmployeeId}");

                if (response.RequestSuccessful == false)
                    _ = Notice.Show(NotificationType.Warning, "Не удалось получить список сотрудников.\nПроверьте связь с сервером.");

                if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") break;

                try
                {
                    employeeTemp = JsonConvert.DeserializeObject<Employee[]>(response.Response);

                    if (employeeTemp.Length > 0)
                        lastEmployeeId = employeeTemp[employeeTemp.Length - 1].Id + 1;

                    // Если получен не пустой список, то добавляет поочерёдно каждую компанию в список
                    foreach (var emp in employeeTemp)
                        employees.Add(emp);
                }
                catch (Exception e) { WriteLog.Error($"{e}"); }
            }

            return employees;
        }

        public static async Task<bool> GetGroups(ObservableCollection<Group> groups)
        {
            groups.Clear();
            Group[] group;

            var response = await GetResponse($"{serverAddress}/group");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список групп.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return false;
            try
            {
                group = JsonConvert.DeserializeObject<Group[]>(response.Response);

                foreach (var gr in group)
                    groups.Add(gr);

                return true;
            }
            catch (Exception e) { WriteLog.Error($"{e}"); return false; }
        }

        public static async Task<bool> GetStatuses(ObservableCollection<Status> statuses)
        {
            statuses.Clear();
            Status[] status;

            var response = await GetResponse($"{serverAddress}/issueStatus");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список статусов заявок.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return false;
            try
            {
                status = JsonConvert.DeserializeObject<Status[]>(response.Response);

                foreach (var st in status)
                    statuses.Add(st);

                return true;
            }
            catch (Exception e) { WriteLog.Error(e.ToString()); return false; }
        }

        public static async Task<bool> GetTypes(ObservableCollection<TaskType> types)
        {
            types.Clear();
            TaskType[] type;

            var response = await GetResponse($"{serverAddress}/issueType");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список типов заявок.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return false;
            try
            {
                type = JsonConvert.DeserializeObject<TaskType[]>(response.Response);

                foreach (var tp in type)
                    types.Add(tp);

                return true;
            }
            catch (Exception e) { WriteLog.Error(e.ToString()); return false; }
        }

        public static async Task<bool> GetPriorities(ObservableCollection<Priority> priorities)
        {
            priorities.Clear();
            Priority[] priority;

            var response = await GetResponse($"{serverAddress}/issuePriority");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить список приоритетов заявок.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return false;
            try
            {
                priority = JsonConvert.DeserializeObject<Priority[]>(response.Response);

                foreach (var pt in priority)
                    priorities.Add(pt);

                return true;
            }
            catch (Exception e) { WriteLog.Error(e.ToString()); return false; }
        }

        public static async Task<bool> GetEmployeePerformance(List<Employee> employees, DateTime dateFrom, DateTime dateTo, string requestType)
        {
            foreach (var emp in employees)
            {
                emp.SolvedTasks = 0;
                emp.SpentedTimeDouble = 0;
                emp.Issues = [];
            }

            EmployeePerformance[] employeePerformanceList;

            var response = await GetResponse(
                $"{serverAddress}/employeePerformance/list?requestType={requestType}&dateFrom={dateFrom:yyyy.MM.dd}&dateTo={dateTo:yyyy.MM.dd}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить данные о продуктивности сотрудников.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return false;
            try
            {
                employeePerformanceList = JsonConvert.DeserializeObject<EmployeePerformance[]>(response.Response);

                foreach (var emp in employeePerformanceList)
                {
                    var e = employees?.Find(e => e.Id == emp.Id);    // Поиск сотрудника по id из общего списка который был загружен ранее
                    int index = employees.IndexOf(e);   // Получение id сотрудника
                    if (index == -1) continue;
                    employees[index].SolvedTasks = emp.SolvedTasks; // Назначает решённые задачи и списанное время по id
                    employees[index].SpentedTimeDouble = emp.SpentedTime;
                    employees[index].Issues = emp.Issues;
                }
                return true;
            }
            catch (Exception e) { WriteLog.Error(e.ToString()); return false; }
        }

        public static async Task<Equipment> GetEquipment(int equipment)
        {
            var response = await GetResponse($"{serverAddress}/equipment?equipmentId={equipment}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось получить данные об оборудовании.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return null;
            try
            {
                return JsonConvert.DeserializeObject<Equipment>(response.Response);

            }
            catch (Exception e) { WriteLog.Error(e.ToString()); return null; }
        }

        public static async Task<UpdateClientInfo> GetUpdateInformationForMaintenanceEntity(int maintenanceEntityId)
        {            
            var response = await GetResponse($"{serverAddress}/maintenanceEntity/update_maintenance_entities?maintenanceEntityId={maintenanceEntityId}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось обновить объект обслуживания.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return null;

            try
            {
                return JsonConvert.DeserializeObject<UpdateClientInfo>(response.Response);
            }
            catch (Exception e) { WriteLog.Error($"{e}"); return null; }
        }

        public static async Task<UpdateClientInfo> GetUpdateInformationForCompany(int companyId)
        {
            var response = await GetResponse($"{serverAddress}/company/update_company?companyId={companyId}");

            if (response.RequestSuccessful == false)
                _ = Notice.Show(NotificationType.Warning, "Не удалось обновить компанию.\nПроверьте связь с сервером.");

            if (string.IsNullOrEmpty(response.Response) || response.Response == "[]") return null;

            try
            {
                return JsonConvert.DeserializeObject<UpdateClientInfo>(response.Response);
            }
            catch (Exception e) { WriteLog.Error($"{e}"); return null; }
        }

        public static async Task<string> GetLastUpdateTime()
        {
            var response = await GetResponse($"{serverAddress}/employeePerformance/time");

            if (string.IsNullOrEmpty(response.Response)) return null;
            try
            {
                var date = Convert.ToDateTime(response.Response.Replace("\"", "").Replace("\\", ""));
                return $"Обновлено: {date:HH:mm}";
            }
            catch (Exception ex) { WriteLog.Warn(ex.ToString()); return null; }
        }

        public static async Task<(bool RequestSuccessful, AuthenticateResponse Response)> RefreshRefreshToken(string refreshToken)
        {
            string link = $"{serverAddress}/user/refresh?refreshToken=";
            string linkEncoded = link + UrlEncode(refreshToken).ToString();
            var response = await PutResponse(linkEncoded);

            if (response.RequestSuccessful == false)
                return (false, null);

            if (string.IsNullOrEmpty(response.Response)) return (false, null);
            try
            {
                return (true, JsonConvert.DeserializeObject<AuthenticateResponse>(response.Response));
            }
            catch (Exception e) { WriteLog.Warn(e.ToString()); return (false, null); }
        }

        public static async Task<(bool RequestSuccessful, AuthenticateResponse Response)> Login(string email, string password)
        {
            string link = $"{serverAddress}/user/login";
            var payload = new AuthenticateRequest(email, password);
            string stringPayload = string.Empty;
            try
            {
                stringPayload = JsonConvert.SerializeObject(payload);
            }
            catch (Exception e) { WriteLog.Warn(e.ToString()); }

            var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            var response = await PostResponse(link, content);

            if (response.RequestSuccessful == false)
                return (false, null);

            if (string.IsNullOrEmpty(response.Response)) return (true, null);
            try
            {
                return (true, JsonConvert.DeserializeObject<AuthenticateResponse>(response.Response));
            }
            catch (Exception e) { WriteLog.Warn(e.ToString()); return (false, null); }
        }

        public static async Task<(bool? RequestSuccessful, string Response)> GetResponse(string link)
        {
            try
            {
                if (!await Auth.CheckAndRefreshToken())
                    return (null, null);

                HttpClient httpClient = new(socketsHandler);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Config.Settings.Token.AccessToken);

                using HttpResponseMessage response = await httpClient.GetAsync(link);

                if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    return (false, null);
                else if (!response.IsSuccessStatusCode)
                    return (true, null);
                else return (true, await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                WriteLog.Warn(e.ToString());
                return (false, null);
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return (false, null);
            }
        }

        public static async Task<(bool RequestSuccessful, string Response)> PostResponse(string link, StringContent content)
        {
            try
            {
                HttpClient httpClient = new(socketsHandler);
                var response = await httpClient.PostAsync(link, content);

                if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    return (false, null);
                else if (!response.IsSuccessStatusCode)
                    return (true, null);
                else
                    return (true, await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                WriteLog.Warn(e.ToString());
                return (false, null);
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return (false, null);
            }
        }

        public static async Task<(bool RequestSuccessful, string Response)> PutResponse(string link, StringContent content = null)
        {
            try
            {
                HttpClient httpClient = new(socketsHandler);
                var response = await httpClient.PutAsync(link, content);
                if (response.StatusCode == System.Net.HttpStatusCode.BadGateway)
                    return (false, null);
                else if (!response.IsSuccessStatusCode)
                    return (true, null);
                else
                    return (true, await response.Content.ReadAsStringAsync());
            }
            catch (HttpRequestException e)
            {
                WriteLog.Warn(e.ToString());
                return (false, null);
            }
            catch (Exception e)
            {
                WriteLog.Warn(e.ToString());
                return (false, null);
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