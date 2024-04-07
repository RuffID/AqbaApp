using AqbaApp.API;
using AqbaApp.Model.OkdeskReport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            CheckedGroups = [];
            CheckedStatuses = [];
            CheckedPriorities = [];
            CheckedTypes = [];
            LoadConfig();

            timer = new(DispatcherPriority.Render)
            {
                Interval = TimeSpan.FromSeconds(300)
            };
            timer.Tick += async (sender, args) =>
            {                
                await GetPerformance();
            };
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
        string pathToBackground;
        List<string> checkedGroups;
        List<string> checkedTypes;
        List<string> checkedStatuses;
        List<string> checkedPriorities;
        DateTime selectedDateFrom;
        DateTime selectedDateTo;
        ObservableCollection<Employee> activeEmployees;
        ObservableCollection<Status> taskStatuses;
        ObservableCollection<Group> groups;
        ObservableCollection<TaskType> taskTypes;
        ObservableCollection<Priority> priorities;
        RelayCommand reportPageLoaded;
        RelayCommand getOpenTasks;
        RelayCommand checkedGroup;
        RelayCommand checkedOpenTasks;
        RelayCommand checkedType;
        RelayCommand checkedPriority;
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

        List<Employee> Employees { get; set; }

        public List<string> CheckedGroups
        {
            get { return checkedGroups; }
            set
            {
                checkedGroups = value;
                OnPropertyChanged(nameof(CheckedGroup));
            }
        }

        public ObservableCollection<Group> Groups
        {
            get { return groups; }
            set
            {
                groups = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Employee> ActiveEmployees
        {
            get { return activeEmployees; }
            set
            {
                activeEmployees = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Status> TaskStatuses
        {
            get { return taskStatuses; }
            set
            {
                taskStatuses = value;
                OnPropertyChanged();                
            }
        }

        public List<string> CheckedStatuses
        {
            get { return checkedStatuses; }
            set
            {
                checkedStatuses = value;
                OnPropertyChanged(nameof(CheckedStatuses));
            }
        }

        public ObservableCollection<TaskType> TaskTypes
        {
            get { return taskTypes; }
            set
            {
                taskTypes = value;
                OnPropertyChanged();
            }
        }

        public List<string> CheckedTypes
        {
            get { return checkedTypes; }
            set
            {
                checkedTypes = value;
                OnPropertyChanged(nameof(CheckedTypes));
            }
        }

        public ObservableCollection<Priority> Priorities
        {
            get { return priorities; }
            set
            {
                priorities = value;
                OnPropertyChanged();
            }
        }

        public List<string> CheckedPriorities
        {
            get { return checkedPriorities; }
            set
            {
                checkedPriorities = value;
                OnPropertyChanged(nameof(CheckedPriorities));
            }
        }

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

        #endregion

        #region [Commands]
        public RelayCommand ReportPageLoaded
        {
            get
            {
                return reportPageLoaded ??= new RelayCommand((o) =>
                {
                    if (Config.Settings.ElectronicQueueMode)
                    {
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
                    await GetPerformance("manual");
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
                    SaveCheckedGroups(); // Сохранение выделенных групп в конфиг                    
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
                    SaveCheckedStatuses(); // Сохранение выделенных статусов в конфиг
                    SaveCheckedPriorities();
                    SaveCheckedTypes();
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
            if (Config.Settings.CheckedGroups != null && Config.Settings.CheckedGroups.Length != 0)
                CheckedGroups = Config.Settings.CheckedGroups.ToList();

            if (Config.Settings.CheckedStatuses != null && Config.Settings.CheckedStatuses.Length != 0)
                CheckedStatuses = Config.Settings.CheckedStatuses.ToList();

            if (Config.Settings.CheckedTypes != null && Config.Settings.CheckedTypes.Length != 0)
                CheckedTypes = Config.Settings.CheckedTypes.ToList();

            if (Config.Settings.CheckedPriorities != null && Config.Settings.CheckedPriorities.Length != 0)
                CheckedPriorities = Config.Settings.CheckedPriorities.ToList();

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

        public void SaveCheckedGroups()
        {
            CheckedGroups?.Clear();

            foreach(var group in Groups)
                if (group.IsChecked)
                    CheckedGroups.Add(group.Name);

            Config.Settings.CheckedGroups = CheckedGroups.ToArray();
        }

        void LoadCheckedGroups()
        {
            if (Config.Settings.CheckedGroups != null && Config.Settings.CheckedGroups.Length != 0)
            {
                foreach (var group in Groups)
                    if (Config.Settings.CheckedGroups.Contains(group.Name))
                        group.IsChecked = true;
                    else group.IsChecked = false;
            }
        }

        public void SaveCheckedStatuses()
        {
            CheckedStatuses?.Clear();

            foreach (var status in TaskStatuses)
                if (status.IsChecked)
                    CheckedStatuses.Add(status.Name);

            Config.Settings.CheckedStatuses = CheckedStatuses.ToArray();
        }

        void LoadCheckedStatuses()
        {
            if (Config.Settings.CheckedStatuses != null && Config.Settings.CheckedStatuses.Length != 0)
            {
                foreach (var status in TaskStatuses)
                    if (Config.Settings.CheckedStatuses.Contains(status.Name))
                        status.IsChecked = true;
                    else status.IsChecked = false;
            }
        }

        public void SaveCheckedTypes()
        {
            CheckedTypes?.Clear();

            foreach (var type in TaskTypes)
                if (type.IsChecked)
                    CheckedTypes.Add(type.Name);

            Config.Settings.CheckedTypes = CheckedTypes.ToArray();
        }

        void LoadCheckedTypes()
        {
            if (Config.Settings.CheckedTypes != null && Config.Settings.CheckedTypes.Length != 0)
            {
                foreach (var type in TaskTypes)
                    if (Config.Settings.CheckedTypes.Contains(type.Name))
                        type.IsChecked = true;
                    else type.IsChecked = false;
            }
        }

        public void SaveCheckedPriorities()
        {
            CheckedPriorities?.Clear();

            foreach (var priority in Priorities)
                if (priority.IsChecked)
                    CheckedPriorities.Add(priority.Name);

            Config.Settings.CheckedPriorities = CheckedPriorities.ToArray();
        }

        void LoadCheckedPriorities()
        {
            if (Config.Settings.CheckedPriorities != null && Config.Settings.CheckedPriorities.Length != 0)
            {
                foreach (var priority in Priorities)
                    if (Config.Settings.CheckedPriorities.Contains(priority.Name))
                        priority.IsChecked = true;
                    else priority.IsChecked = false;
            }
        }

        async Task CheckDictionaries()
        {
            if (Employees?.Count <= 0)
            {
                var temp = (await Request.GetEmployees())?.ToList();
                if (temp == null || temp.Count == 0)
                {
                    GettingTaskInRun = true;
                    return;
                }
                Employees = temp;
            }

            if (Groups.Count <= 0)
            {
                if (!await Request.GetGroups(Groups))
                {
                    GettingTaskInRun = true;
                    return;
                }
                if (CheckedGroups != null && CheckedGroups.Count <= 0)
                    SaveCheckedGroups();
                else LoadCheckedGroups();
            }

            if (TaskStatuses.Count <= 0)
            {
                if (!await Request.GetStatuses(TaskStatuses))
                {
                    GettingTaskInRun = true;
                    return;
                }
                if (TaskStatuses != null && TaskStatuses.Count <= 0)
                    SaveCheckedStatuses();
                else LoadCheckedStatuses();
            }

            if (TaskTypes.Count <= 0)
            {
                if (!await Request.GetTypes(TaskTypes))
                {
                    GettingTaskInRun = true;
                    return;
                }
                if (TaskTypes != null && TaskTypes.Count <= 0)
                    SaveCheckedTypes();
                else LoadCheckedTypes();
            }

            if (Priorities.Count <= 0)
            {
                if (!await Request.GetPriorities(Priorities))
                {
                    GettingTaskInRun = true;
                    return;
                }
                if (Priorities != null && Priorities.Count <= 0)
                    SaveCheckedPriorities();
                else LoadCheckedPriorities();
            }
        }

        async Task GetPerformance(string requestType = "auto")
        {
            timer.Start();
            GettingTaskInRun = false;
            if (Config.Settings.ElectronicQueueMode)
            {
                SelectedDateTo = DateTime.Now;
                SelectedDateFrom = DateTime.Now;
            }

            await CheckDictionaries();
            SetOpenStatus();

            if (Employees?.Count > 0 && Groups.Count > 0 && TaskTypes.Count > 0 && Priorities.Count > 0 && TaskStatuses.Count > 0)
            {
                await Request.GetEmployeePerformance(Employees, SelectedDateFrom, SelectedDateTo, requestType);

                CheckGroup();
                CheckOpenTasks();
                SortEmployees();                
            }
            LastUpdateTime = await Request.GetLastUpdateTime();
            GettingTaskInRun = true;
        }

        void SortEmployees()
        {
            // Очистка списка активных сотрудников
            ActiveEmployees?.Clear();
            // Сортировка всех сотрудников по решённым заявкам по уменьшению
            if (Employees != null)
            {
                Employees.Sort();
                foreach (var employee in Employees)
                {
                    // Если сотрудник активен, то есть имеет решённые/открытые заявки или списанное время, то добавить в активные
                    if ((employee.OpenTasks != 0 || employee.SolvedTasks != 0 || employee.SpentedTimeDouble != 0) && employee.IsSelected == true)
                        ActiveEmployees?.Add(employee);
                    else
                        employee.IsSelected = false;
                }
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
                    if (Groups[i]?.EmployeesId.Contains(employee.Id) == true && Groups[i].IsChecked == true)
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

                // Если количество является не null, то записывает его в открытые заявки сотрудника
                if (count != null && count > 0)
                    employee.OpenTasks += (int)count;
            }
        }

        void SetOpenStatus()
        {
            foreach (var status in TaskStatuses)
            {
                switch (status.Code)
                {
                    case "opened":
                        status.IsChecked = true; break;
                    case "completed_outdor":
                        status.IsChecked = true; break;
                    case "injob":
                        status.IsChecked = true; break;
                }
            }            
        }

        #endregion
    }
}