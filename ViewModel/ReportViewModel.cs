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
            Groups = [];
            CheckedGroups = [];
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
        readonly DispatcherTimer timer;
        bool gettingTaskInRun = true;
        string pathToBackground;
        List<string> checkedGroups;        
        DateTime selectedDateFrom;
        DateTime selectedDateTo;
        ObservableCollection<Employee> activeEmployees;
        ObservableCollection<Status> taskStatuses;
        ObservableCollection<Group> groups;
        ObservableCollection<TaskType> taskTypes;
        RelayCommand reportPageLoaded;
        RelayCommand getOpenTasks;
        RelayCommand checkedGroup;
        /*RelayCommand leftClickStatusSelectButton;
        RelayCommand rigthClickStatusSelectButton;
        RelayCommand leftClickTypeSelectButton;
        RelayCommand rigthClickTypeSelectButton;*/
        RelayCommand leftClickGroupSelectButton;
        RelayCommand rigthClickGroupSelectButton;

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

        public ObservableCollection<TaskType> TaskTypes
        {
            get { return taskTypes; }
            set
            {
                taskTypes = value;
                OnPropertyChanged();
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

        #endregion

        #region [Commands]
        public RelayCommand ReportPageLoaded
        {
            get
            {
                return reportPageLoaded ??= new RelayCommand(async (o) =>
                {
                    if (Employees?.Count <= 0)
                    {
                        var employees = await Request.GetEmployees();
                        if (employees == null || employees.Count == 0)
                            return;

                        Employees = employees.ToList();
                    }

                    if (Groups.Count == 0)
                    {
                        await Request.GetGroups(Groups);
                        if (CheckedGroups != null && CheckedGroups.Count == 0)
                            SaveCheckedGroups();
                        else LoadCheckedGroups();
                    }

                    /*if (TaskStatuses.Count == 0)
                    {
                        await OkdeskApi.GetStatuses(TaskStatuses);
                        SetOpenStatus();                        
                    }

                    if (TaskTypes.Count == 0)
                        await OkdeskApi.GetTypes(TaskTypes);*/
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

        /*public RelayCommand LeftClickStatusSelectButton
        {
            get
            {
                return leftClickStatusSelectButton ??= new RelayCommand((o) =>
                {
                    foreach (var status in TaskStatuses)
                        status.IsChecked = true;
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
                });
            }
        }*/

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

        #endregion

        #region [Methods]

        void LoadConfig()
        {
            if (Config.Settings.CheckedGroups != null && Config.Settings.CheckedGroups.Length != 0)
                CheckedGroups = Config.Settings.CheckedGroups.ToList();

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

        async Task GetPerformance(string requestType = "auto")
        {            
            GettingTaskInRun = false;
            Employees?.Clear();
            timer.Stop();
            if (Config.Settings.ElectronicQueueMode)
            {
                SelectedDateTo = DateTime.Now;
                SelectedDateFrom = DateTime.Now;
            }

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

            if (Employees?.Count > 0 && Groups.Count > 0)
            {
                await Request.GetEmployeePerformance(Employees, SelectedDateFrom, SelectedDateTo, requestType);

                CheckGroup();
                SortEmployees();
                timer.Start();
            }
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
                foreach (var employee in Employees.Where(e => e.Active))
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
            foreach (var employee in Employees.Where(e => e.Active))
            {
                // Проходит по всем чекбоксам групп
                for (var i = 0; i < Groups.Count; i++)
                {
                    // Если сотрудник состоит в группе и она выделена в чекбоксе, то
                    //if (Groups[i]?.Employees?.Contains(employee) == true && Groups[i].IsChecked == true)
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