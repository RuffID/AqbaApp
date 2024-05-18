using AqbaApp.API;
using AqbaApp.Dto;
using AqbaApp.Interfaces;
using AqbaApp.Model.OkdeskReport;
using AqbaServer.Models.OkdeskPerformance;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AqbaApp.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        public ReportViewModel()
        {
            SelectedDateTo = DateTime.Now;
            SelectedDateFrom = DateTime.Now;
            Employees = [];
            ActiveEmployees = [];
            TaskStatuses = [];
            TaskTypes = [];
            Priorities = [];
            Groups = [];
            GroupEmployeeConnections = [];
            Priorities.CollectionChanged += OnCollectionChanged;
            TaskStatuses.CollectionChanged += OnCollectionChanged;
            TaskTypes.CollectionChanged += OnCollectionChanged;
            Groups.CollectionChanged += OnCollectionChanged;
            LoadConfig();
            timer = new(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(300) };
            timer.Tick += async (sender, args) => await GetPerformance(requestType: "auto");
        }

        #region [Variables]

        string headerBackgroundColor;
        string fullNameBackgroundColor;
        string spendedTimeBackgroundColor;
        string solvedTasksBackgroundColor;
        string openTasksBackgroundColor;
        string lastUpdateTime;
        readonly DispatcherTimer timer;
        bool gettingTaskInRun = true;
        bool hideUselessEmployees;
        bool hideWithoutWrittenOffTime;
        bool hideEmployeesWithoutOpenIssues;
        string pathToBackground;
        DateTime selectedDateFrom;
        DateTime selectedDateTo;
        ObservableCollection<Employee> activeEmployees;
        ObservableCollection<IOkdeskDictionary> groups;
        ObservableCollection<IOkdeskDictionary> taskStatuses;
        ObservableCollection<IOkdeskDictionary> taskTypes;
        ObservableCollection<IOkdeskDictionary> priorities;
        RelayCommand reportPageLoaded;
        RelayCommand getOpenTasks;
        RelayCommand checkedGroup;
        RelayCommand checkedOpenTasks;
        RelayCommand leftClickStatusSelectButton;
        RelayCommand rigthClickStatusSelectButton;
        RelayCommand leftClickTypeSelectButton;
        RelayCommand rigthClickTypeSelectButton;
        RelayCommand leftClickGroupSelectButton;
        RelayCommand rigthClickGroupSelectButton;
        RelayCommand leftClickPrioritySelectButton;
        RelayCommand rigthClickPrioritySelectButton;

        #endregion

        #region [Collections]

        private ICollection<GroupEmployee> GroupEmployeeConnections { get; set; }

        private List<Employee> Employees { get; set; }

        public ObservableCollection<Employee> ActiveEmployees
        {
            get { return activeEmployees; }
            set
            {
                activeEmployees = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IOkdeskDictionary> TaskStatuses { get; set; }

        public ObservableCollection<IOkdeskDictionary> TaskTypes { get; set; }

        public ObservableCollection<IOkdeskDictionary> Priorities {  get; set; }

        public ObservableCollection<IOkdeskDictionary> Groups {  get; set; }

        public DateTime SelectedDateFrom
        {
            get { return selectedDateFrom; }
            set
            {
                selectedDateFrom = value;
                OnPropertyChanged();
            }
        }

        public DateTime SelectedDateTo
        {
            get { return selectedDateTo; }
            set
            {
                selectedDateTo = value;
                OnPropertyChanged();
            }
        }

        public string LastUpdateTime
        {
            get { return lastUpdateTime; }
            set
            {
                lastUpdateTime = value;
                OnPropertyChanged();
            }
        }

        public bool GettingTaskInRun
        {
            get { return gettingTaskInRun; }
            set
            {
                gettingTaskInRun = value;
                OnPropertyChanged();
            }
        }

        public string PathToBackground
        {
            get { return pathToBackground; }
            set
            {
                pathToBackground = value;
                OnPropertyChanged();
            }
        }        

        public string HeaderBackgroundColor
        {
            get { return headerBackgroundColor; }
            set
            {
                headerBackgroundColor = value;
                Config.Settings.HeaderBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public string FullNameBackgroundColor
        {
            get { return fullNameBackgroundColor; }
            set
            {
                fullNameBackgroundColor = value;
                Config.Settings.FullNameBackgroundColor = value;
                OnPropertyChanged();
            }
        }
        
        public string SolvedTasksBackgroundColor
        {
            get { return solvedTasksBackgroundColor; }
            set
            {
                solvedTasksBackgroundColor = value;
                Config.Settings.SolvedTasksBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public string SpendedTimeBackgroundColor
        {
            get { return spendedTimeBackgroundColor; }
            set
            {
                spendedTimeBackgroundColor = value;
                Config.Settings.SpendedTimeBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public string OpenTasksBackgroundColor
        {
            get { return openTasksBackgroundColor; }
            set
            {
                openTasksBackgroundColor = value;
                Config.Settings.OpenTasksBackgroundColor = value;
                OnPropertyChanged();
            }
        }

        public bool HideEmployeesWithoutSolvedIssues
        {
            get { return hideUselessEmployees; }
            set
            {
                hideUselessEmployees = value;
                Config.Settings.HideEmployeesWithoutSolvedIssues = value;
                OnPropertyChanged(nameof(HideEmployeesWithoutSolvedIssues));
                CheckGroup();
                SortEmployees();
            }
        }

        public bool HideEmployeesWithoutWrittenOffTime
        {
            get { return hideWithoutWrittenOffTime; }
            set
            {
                hideWithoutWrittenOffTime = value;
                Config.Settings.HideEmployeesWithoutWrittenOffTime = value;
                OnPropertyChanged(nameof(HideEmployeesWithoutWrittenOffTime));
                CheckGroup();
                SortEmployees();
            }
        }

        public bool HideEmployeesWithoutOpenIssues
        {
            get { return hideEmployeesWithoutOpenIssues; }
            set
            {
                hideEmployeesWithoutOpenIssues = value;
                Config.Settings.HideEmployeesWithoutOpenIssues = value;
                OnPropertyChanged(nameof(HideEmployeesWithoutOpenIssues));
                CheckGroup();
                SortEmployees();
            }
        }

        #endregion

        #region [Commands]
        public RelayCommand ReportPageLoaded
        {
            get
            {
                return reportPageLoaded ??= new RelayCommand(async (o) =>
                {
                    if (Config.Settings.ElectronicQueueMode)
                    {
                        await CheckDictionaries();
                        timer.Start();
                    }
                });
            }
        }

        public RelayCommand GetOpenTasks
        {
            get
            {
                return getOpenTasks ??= new RelayCommand(async (o) =>
                {
                    await GetPerformance(requestType: "manual");
                });
            }
        }
        
        public RelayCommand CheckedGroup
        {
            get
            {
                return checkedGroup ??= new RelayCommand((o) =>
                {
                    CheckGroup(); // Проверка выделенных групп
                    SortEmployees(); // Сортировка сотрудников
                    //SaveCheckedGroups(); // Сохранение выделенных групп в конфиг                    
                });
            }
        }

        public RelayCommand CheckedOpenTasks
        {
            get
            {
                return checkedOpenTasks ??= new RelayCommand((o) =>
                {
                    CheckOpenTasks(); // Проверка открытых заявок
                    SortEmployees(); // Сортировка сотрудников
                    //SaveCheckedStatuses(); // Сохранение выделенных статусов в конфиг
                    //SaveCheckedTypes();
                });
            }
        }

        public RelayCommand LeftClickStatusSelectButton
        {
            get
            {
                return leftClickStatusSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var status in TaskStatuses)
                        status.IsChecked = true;
                    CheckOpenTasks();
                    SortEmployees();
                });
            }
        }

        public RelayCommand RightClickStatusSelectButton
        {
            get
            {
                return rigthClickStatusSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var status in TaskStatuses)
                        status.IsChecked = false;
                    CheckOpenTasks();
                    SortEmployees();
                });
            }
        }

        public RelayCommand LeftClickTypeSelectButton
        {
            get
            {
                return leftClickTypeSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var type in TaskTypes)
                        type.IsChecked = true;
                    CheckOpenTasks();
                    SortEmployees();
                });
            }
        }

        public RelayCommand RightClickTypeSelectButton
        {
            get
            {
                return rigthClickTypeSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var type in TaskTypes)
                        type.IsChecked = false;
                    CheckOpenTasks();
                    SortEmployees();
                });
            }
        }

        public RelayCommand LeftClickGroupSelectButton
        {
            get
            {
                return leftClickGroupSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var group in Groups)
                        group.IsChecked = true;
                    CheckGroup();
                    SortEmployees();                    
                });
            }
        }

        public RelayCommand RightClickGroupSelectButton
        {
            get
            {
                return rigthClickGroupSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var group in Groups)
                        group.IsChecked = false;
                    CheckGroup();
                    SortEmployees();
                });
            }
        }

        public RelayCommand LeftClickPrioritySelectButton
        {
            get
            {
                return leftClickPrioritySelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var prior in Priorities)
                        prior.IsChecked = true;
                    CheckOpenTasks();
                    SortEmployees();
                });
            }
        }

        public RelayCommand RightClickPrioritySelectButton
        {
            get
            {
                return rigthClickPrioritySelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var prior in Priorities)
                        prior.IsChecked = false;
                    CheckOpenTasks();
                    SortEmployees();
                });
            }
        }

        #endregion

        #region [Methods]

        void LoadConfig()
        {
            if (Config.Cache.Groups != null)
                foreach (var group in Config.Cache.Groups)
                    Groups.Add(new Group(group));

            if (Config.Cache.Statuses != null)
                foreach (var status in Config.Cache.Statuses)
                    TaskStatuses.Add(new Status(status));

            if (Config.Cache.Types != null)
                foreach (var type in Config.Cache.Types)
                    TaskTypes.Add(new TaskType(type));

            if (Config.Cache.Priorities != null)
                foreach (var priority in Config.Cache.Priorities) 
                    Priorities.Add(new Priority(priority));

            HideEmployeesWithoutSolvedIssues = Config.Settings.HideEmployeesWithoutSolvedIssues;

            HideEmployeesWithoutWrittenOffTime = Config.Settings.HideEmployeesWithoutWrittenOffTime;

            HideEmployeesWithoutOpenIssues = Config.Settings.HideEmployeesWithoutOpenIssues;

            if (string.IsNullOrEmpty(Config.Settings.PathToBackground))
                PathToBackground = "View/Resources/Background/White.png";
            else PathToBackground = Config.Settings.PathToBackground;

            if (Config.Settings.HeaderFontSize > 72 || Config.Settings.HeaderFontSize < 8)
                Config.Settings.HeaderFontSize = 18;

            if (Config.Settings.FullNameFontSize > 72 || Config.Settings.FullNameFontSize < 8)
                Config.Settings.FullNameFontSize = 16;

            if  (Config.Settings.SolvedTasksFontSize > 72 || Config.Settings.SolvedTasksFontSize < 8)
                Config.Settings.SolvedTasksFontSize = 16;

            if (Config.Settings.SpendedTimeFontSize > 72 || Config.Settings.SpendedTimeFontSize < 8)
                Config.Settings.SpendedTimeFontSize = 16;

            if (Config.Settings.OpenTasksFontSize > 72 || Config.Settings.OpenTasksFontSize < 8)
                Config.Settings.OpenTasksFontSize = 16;

            if (string.IsNullOrEmpty(Config.Settings.HeaderBackgroundColor))            
                HeaderBackgroundColor = "#000000";
            else HeaderBackgroundColor = Config.Settings.HeaderBackgroundColor;

            if (string.IsNullOrEmpty(Config.Settings.SolvedTasksBackgroundColor))
                SolvedTasksBackgroundColor = "#000000";
            else SolvedTasksBackgroundColor = Config.Settings.SolvedTasksBackgroundColor;

            if (string.IsNullOrEmpty(Config.Settings.FullNameBackgroundColor))
                FullNameBackgroundColor = "#000000";
            else FullNameBackgroundColor = Config.Settings.FullNameBackgroundColor;

            if (string.IsNullOrEmpty(Config.Settings.SpendedTimeBackgroundColor))
                SpendedTimeBackgroundColor = "#000000";
            else SpendedTimeBackgroundColor = Config.Settings.SpendedTimeBackgroundColor;

            if (string.IsNullOrEmpty(Config.Settings.OpenTasksBackgroundColor))
                OpenTasksBackgroundColor = "#000000";
            else OpenTasksBackgroundColor = Config.Settings.OpenTasksBackgroundColor;
        }        

        public void SaveBackgroundPath(string path)
        {
            PathToBackground = path;
            Config.Settings.PathToBackground = PathToBackground;
        }

        async Task GetPerformance(string requestType)
        {
            GettingTaskInRun = false;

            if (!timer.IsEnabled)
                await CheckDictionaries();

            timer.Start();

            if (Config.Settings.ElectronicQueueMode)
            {
                SelectedDateTo = DateTime.Now;
                SelectedDateFrom = DateTime.Now;
            }

            if (Employees?.Count != 0 && Groups.Count != 0 && TaskTypes.Count != 0 && Priorities.Count != 0 && TaskStatuses.Count != 0)
            {
                await Request.GetEmployeePerformance(Employees, SelectedDateFrom, SelectedDateTo, requestType);

                CheckGroup();
                CheckOpenTasks();
                SortEmployees();
            }

            LastUpdateTime = await Request.GetLastUpdateTime();
            GettingTaskInRun = true;
        }

        async Task CheckDictionaries()
        {
            var connections = await Request.GetDictionaries<GroupEmployee>(apiEndpoint: "employee/group_employee");
            WriteLog.Info($"Количество связей сотрудников: {connections.Count}");
            UpdateCollection(connections, GroupEmployeeConnections);

            var employees = await Request.GetCollectionFromAPI<Employee>("employee");
            UpdateCollection(employees, Employees);
            WriteLog.Info($"Количество сотрудников: {employees.Count}");

            var groups = await Request.GetDictionaries<Group>(apiEndpoint: "group") as ICollection<IOkdeskDictionary>;
            UpdateOrRemoveDictionary(groups, Groups);

            var statuses = await Request.GetDictionaries<Status>(apiEndpoint: "issueStatus") as ICollection<IOkdeskDictionary>;
            UpdateOrRemoveDictionary(statuses, TaskStatuses);

            var taskTypes = await Request.GetDictionaries<TaskType>(apiEndpoint: "issueType") as ICollection<IOkdeskDictionary>;
            UpdateOrRemoveDictionary(taskTypes, TaskTypes);

            var priorities = await Request.GetDictionaries<Priority>(apiEndpoint: "issuePriority") as ICollection<IOkdeskDictionary>;
            UpdateOrRemoveDictionary(priorities, Priorities);
        }

        static void UpdateCollection<T>(ICollection<T> collectionFromAPI, ICollection<T> collectionFromCache)
        {
            if (collectionFromAPI == null) return;

            foreach (var item in collectionFromAPI)
                collectionFromCache.Add(item);
        }

        static void UpdateOrRemoveDictionary(ICollection<IOkdeskDictionary> collectionFromAPI, ObservableCollection<IOkdeskDictionary> collectionInCache)
        {
            if (collectionFromAPI == null) return;

            foreach (var itemFromAPI in collectionFromAPI)
            {
                var itemFromCache = collectionInCache.FirstOrDefault(p => p.Id == itemFromAPI.Id);

                if (itemFromCache == null) 
                    collectionInCache.Add(itemFromAPI);
                else 
                    itemFromCache.UpdateWithoutChecked(itemFromAPI);
            }

            // Если в кеше есть элемент, который был удалён/либо его нет в API, то удаляет его
            foreach (var itemFromCache in collectionInCache.ToList())
            {
                var itemFromApi = collectionFromAPI.FirstOrDefault(p => p.Id == itemFromCache.Id);

                if (itemFromApi == null) 
                    collectionInCache.Remove(itemFromCache);
            }
        }        

        void SortEmployees()
        {
            // Очистка списка активных сотрудников
            ActiveEmployees?.Clear();
            // Сортировка всех сотрудников по решённым заявкам по уменьшению
            if (Employees == null) return;

            Employees.Sort();
            foreach (var employee in Employees)
            {
                if (HideEmployeesWithoutSolvedIssues && employee.SolvedIssues == 0)
                {
                    employee.IsSelected = false;
                    continue;
                }

                if (HideEmployeesWithoutWrittenOffTime && employee.SpentedTime == 0)
                {
                    employee.IsSelected = false;
                    continue;
                }

                if (HideEmployeesWithoutOpenIssues && employee.OpenTasks == 0)
                {
                    employee.IsSelected = false;
                    continue;
                }

                // Если сотрудник активен, то есть имеет текущие заявки или списанное время, то добавить в активные
                if ((employee.SolvedIssues != 0 || employee.OpenTasks != 0 || employee.SpentedTime != 0) && employee.IsSelected == true)
                {
                    ActiveEmployees?.Add(employee);
                }
                else
                    employee.IsSelected = false;
            }
        }

        void CheckGroup()
        {
            // Находит всех активных сотрудников
            foreach (var employee in Employees)
            {
                // Проходит по всем чекбоксам групп
                for (var i = 0; i < Groups.Count; i++)
                {
                    // Если сотрудник состоит в группе и она выделена в чекбоксе, то
                    
                    var employeeId = GroupEmployeeConnections.FirstOrDefault(e => e.Id == employee.Id);
                    var connection = employeeId.Groups.Any(g => g == Groups[i].Id);

                    if (connection == true && Groups[i].IsChecked == true)
                    {
                        // Если сотрудника отключили (убрав чекбокс с группы), а теперь включили назад,
                        // то вернуть "выделение"
                        if (employee.IsSelected == false)
                            employee.IsSelected = true;
                        break;
                    }

                    // Эта проверка нужна чтобы цикл пробежался по всем группам сотрудника и если ни одна не выделена, то снимает выделение
                    if (i >= Groups.Count - 1)
                        employee.IsSelected = false;
                }
            }
        }

        void CheckOpenTasks()
        {
            foreach (var employee in Employees)
            {
                employee.OpenTasks = 0;
                int? count = 0;

                // Сначала получает список всех заявок по сотруднику, которые имеют "статус" выбранный (isChecked) в приложении галочкой
                // После ищет список заявок который помимо статуса имеют выбранный "тип"
                // Предпослений "Where" получает итоговый список заявок которые имеют выбранный "приоритет"
                // Count считает количество найденных заявок
                count = employee?.Issues
                    ?.Where( i => i?.StatusId == TaskStatuses?.FirstOrDefault( ts => ts.IsChecked && ts.Id == i?.StatusId )?.Id )
                    ?.Where( i => i?.TypeId == TaskTypes?.FirstOrDefault( tt => tt.IsChecked && tt.Id == i?.TypeId )?.Id )
                    ?.Where( i => i?.PriorityId == Priorities?.FirstOrDefault( p => p.IsChecked && p.Id == i?.PriorityId )?.Id)
                    ?.Count();

                // Если количество является не null, то записывает его в открытые(текущие) заявки сотрудника
                if (count != null && count > 0)
                    employee.OpenTasks += (int)count;
            }
        }
        
        static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += ItemPropertyChanged;

                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        if (item is Priority priority)
                        {
                            if (Config.Cache.Priorities.Any(pInCache => pInCache.Id == priority.Id)) 
                                continue;
                            Config.Cache.Priorities.Add(new Priority() { Id = priority.Id, Name = priority.Name, IsChecked = priority.IsChecked });
                        }
                        else if (item is Status status)
                        {
                            if (Config.Cache.Statuses.Any(sInCache => sInCache.Id == status.Id)) 
                                continue;
                            Config.Cache.Statuses.Add(new Status() { Id = status.Id, Name = status.Name, IsChecked = status.IsChecked });
                        }
                        else if (item is TaskType type)
                        {
                            if (Config.Cache.Types.Any(tInCache => tInCache.Id == type.Id)) 
                                continue;
                            Config.Cache.Types.Add(new TaskType() { Id = type.Id, Name = type.Name, IsChecked = type.IsChecked });
                        }
                        else if (item is Group group)
                        {
                            if (Config.Cache.Groups.Any(gInCache => gInCache.Id == group.Id)) 
                                continue;
                            Config.Cache.Groups.Add(new Group() { Id = group.Id, Name = group.Name, IsChecked = group.IsChecked });
                        }
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= ItemPropertyChanged;

                if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    foreach (var item in e.OldItems)
                    {
                        if (item is Priority priority)
                        {
                            var priorityFroDelete = Config.Cache.Priorities?.FirstOrDefault(pInCache => pInCache.Id == priority.Id);
                            Config.Cache.Priorities?.Remove(priorityFroDelete);
                        }
                        else if (item is Status status)
                        {
                            var statusFroDelete = Config.Cache.Statuses?.FirstOrDefault(sInCache => sInCache.Id == status.Id);
                            Config.Cache.Statuses?.Remove(statusFroDelete);
                        }
                        else if (item is TaskType type)
                        {
                            var typeFroDelete = Config.Cache.Types?.FirstOrDefault(tInCache => tInCache.Id == type.Id);
                            Config.Cache.Types?.Remove(typeFroDelete);
                        }
                        else if (item is Group group)
                        {
                            var groupFroDelete = Config.Cache.Groups?.FirstOrDefault(tInCache => tInCache.Id == group.Id);
                            Config.Cache.Groups?.Remove(groupFroDelete);
                        }
                    }
                }
            }
        }

        static void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Priority priority)
            {
                var priorityInCache = Config.Cache.Priorities?.FirstOrDefault(p => p.Id == priority.Id);

                priorityInCache?.Update(priority);
            }
            else if (sender is Status status)
            {
                var statusInCache = Config.Cache.Statuses?.FirstOrDefault(p => p.Id == status.Id);

                statusInCache?.Update(status);
            }
            else if (sender is TaskType type)
            {
                var typeInCache = Config.Cache.Types?.FirstOrDefault(p => p.Id == type.Id);

                typeInCache?.Update(type);
            }
            else if (sender is Group group)
            {
                var groupInCache = Config.Cache.Groups?.FirstOrDefault(p => p.Id == group.Id);

                groupInCache?.Update(group);
            }
        }

        #endregion
    }
}