using AqbaApp.Core;
using AqbaApp.Core.Api;
using AqbaApp.Interfaces;
using AqbaApp.Interfaces.Service;
using AqbaApp.Model.Client;
using AqbaApp.Model.OkdeskReport;
using AqbaApp.Service.Client;
using AqbaApp.Service.OkdeskEntity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AqbaApp.ViewModel
{
    public class ReportViewModel : NotifyProperty
    {
        #region [Variables]

        private readonly ILogger<ReportViewModel> _logger;
        private readonly GetItemService _request;
        private readonly SettingService<MainSettings> _mainSettings;
        private readonly SettingService<EntitiesCache> _entitiesCache;
        private readonly Immutable _immutable;
        private readonly ReportService reportService;
        private readonly EmployeeService employeeService;
        private readonly string mainServerLink;
        readonly DispatcherTimer timer;
        private bool gettingTaskInRun;
        DateTime selectedDateFrom;
        DateTime selectedDateTo;

        #endregion

        public ReportViewModel(SettingService<MainSettings> mainSettings, SettingService<EntitiesCache> entitiesCache, Immutable immutable, ILoggerFactory logger, IHttpClientFactory httpClient, INavigationService navigate)
        {
            _logger = logger.CreateLogger<ReportViewModel>();
            _mainSettings = mainSettings;
            _entitiesCache = entitiesCache;
            _immutable = immutable;
            _request = new(logger, httpClient, navigate, mainSettings);
            reportService = new (mainSettings, immutable, _request);
            employeeService = new (mainSettings, _request, immutable);
            mainServerLink = $"{_mainSettings.Settings.ServerAddress}/{_immutable.ApiMainEndpoint}";
            SelectedDateTo = DateTime.Now;
            SelectedDateFrom = DateTime.Now;
            gettingTaskInRun = true;

            // Загрузка коллекций с параметрами из конфига
            TaskStatuses = new(_entitiesCache.Settings.Statuses);
            TaskTypes = new(_entitiesCache.Settings.Types);
            Priorities = new(_entitiesCache.Settings.Priorities);
            Groups = new(_entitiesCache.Settings.Groups);

            Priorities.CollectionChanged += OnCollectionChanged;
            TaskStatuses.CollectionChanged += OnCollectionChanged;
            TaskTypes.CollectionChanged += OnCollectionChanged;
            Groups.CollectionChanged += OnCollectionChanged;
            timer = new(DispatcherPriority.Render) { Interval = TimeSpan.FromSeconds(300) };
#if !DEBUG
            timer.Tick += async (sender, args) => await GetPerformance();
#endif
        }

        #region [Collections]

        private List<EmployeeGroup> GroupEmployeeConnections { get; set; } = [];

        private List<Employee> Employees { get; set; } = [];

        public ObservableCollection<Employee> ActiveEmployees { get; set; } = [];

        public ObservableCollection<IOkdeskDictionary> TaskStatuses { get; set; }

        public ObservableCollection<IOkdeskDictionary> TaskTypes { get; set; }

        public ObservableCollection<IOkdeskDictionary> Priorities {  get; set; }

        public ObservableCollection<IOkdeskDictionary> Groups {  get; set; }

        public DateTime SelectedDateFrom
        {
            // Возвращает только дату, без времени
            get => selectedDateFrom.Date;
            set
            {
                selectedDateFrom = value;
                OnPropertyChanged(nameof(SelectedDateFrom));
            }
        }

        public DateTime SelectedDateTo
        {
            get => selectedDateTo.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            set
            {
                selectedDateTo = value;
                OnPropertyChanged(nameof(SelectedDateTo));
            }
        }        

        public bool GettingTaskInRun
        {
            get => gettingTaskInRun;
            set
            {
                gettingTaskInRun = value;
                OnPropertyChanged(nameof(GettingTaskInRun));
            }
        }

        public bool ElectronicQueueMode
        {
            get => _mainSettings.Settings.ElectronicQueueMode;
            set
            {
                _mainSettings.Settings.ElectronicQueueMode = value;
                OnPropertyChanged(nameof(ElectronicQueueMode));
            }
        }

        public string PathToBackground
        {
            get => _mainSettings.Settings.PathToBackground;
            set
            {
                _mainSettings.Settings.PathToBackground = value;
                OnPropertyChanged(nameof(PathToBackground));
            }
        }

        public int FullNameFontSize
        {
            get => _mainSettings.Settings.FullNameFontSize;
            set
            {
                _mainSettings.Settings.FullNameFontSize = value;
                OnPropertyChanged(nameof(FullNameFontSize));
            }
        }

        public int SolvedTasksFontSize
        {
            get => _mainSettings.Settings.SolvedTasksFontSize;
            set
            {
                _mainSettings.Settings.SolvedTasksFontSize = value;
                OnPropertyChanged(nameof(SolvedTasksFontSize));
            }
        }

        public int OpenTasksFontSize
        {
            get => _mainSettings.Settings.OpenTasksFontSize;
            set
            {
                _mainSettings.Settings.OpenTasksFontSize = value;
                OnPropertyChanged(nameof(OpenTasksFontSize));
            }
        }

        public int SpendedTimeFontSize
        {
            get => _mainSettings.Settings.SpendedTimeFontSize;
            set
            {
                _mainSettings.Settings.SpendedTimeFontSize = value;
                OnPropertyChanged(nameof(SpendedTimeFontSize));
            }
        }

        public int HeaderFontSize
        {
            get => _mainSettings.Settings.HeaderFontSize;
            set
            {
                _mainSettings.Settings.HeaderFontSize = value;
                OnPropertyChanged(nameof(HeaderFontSize));
            }
        }

        public string HeaderBackgroundColor
        {
            get => _mainSettings.Settings.HeaderBackgroundColor; 
            set
            {
                _mainSettings.Settings.HeaderBackgroundColor = value;
                OnPropertyChanged(nameof(HeaderBackgroundColor));
            }
        }

        public string FullNameBackgroundColor
        {
            get => _mainSettings.Settings.FullNameBackgroundColor;
            set
            {
                _mainSettings.Settings.FullNameBackgroundColor = value;
                OnPropertyChanged(nameof(FullNameBackgroundColor));
            }
        }
        
        public string SolvedTasksBackgroundColor
        {
            get => _mainSettings.Settings.SolvedTasksBackgroundColor;
            set
            {
                _mainSettings.Settings.SolvedTasksBackgroundColor = value;
                OnPropertyChanged(nameof(SolvedTasksBackgroundColor));
            }
        }

        public string SpendedTimeBackgroundColor
        {
            get => _mainSettings.Settings.SpendedTimeBackgroundColor; 
            set
            {
                _mainSettings.Settings.SpendedTimeBackgroundColor = value;
                OnPropertyChanged(nameof(SpendedTimeBackgroundColor));
            }
        }

        public string OpenTasksBackgroundColor
        {
            get => _mainSettings.Settings.OpenTasksBackgroundColor;
            set
            {
                _mainSettings.Settings.OpenTasksBackgroundColor = value;
                OnPropertyChanged(nameof(OpenTasksBackgroundColor));
            }
        }

        public bool HideEmployeesWithoutSolvedIssues
        {
            get => _mainSettings.Settings.HideEmployeesWithoutSolvedIssues;
            set
            {
                _mainSettings.Settings.HideEmployeesWithoutSolvedIssues = value;
                OnPropertyChanged(nameof(HideEmployeesWithoutSolvedIssues));
                CheckGroup();
                SortEmployees();
            }
        }

        public bool HideEmployeesWithoutWrittenOffTime
        {
            get => _mainSettings.Settings.HideEmployeesWithoutWrittenOffTime;
            set
            {
                _mainSettings.Settings.HideEmployeesWithoutWrittenOffTime = value;
                OnPropertyChanged(nameof(HideEmployeesWithoutWrittenOffTime));
                CheckGroup();
                SortEmployees();
            }
        }

        public bool HideEmployeesWithoutOpenIssues
        {
            get => _mainSettings.Settings.HideEmployeesWithoutOpenIssues;
            set
            {
                _mainSettings.Settings.HideEmployeesWithoutOpenIssues = value;
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
                return new RelayCommand(async (o) =>
                {   
                    // В режиме электронной очереди кнопка на получение обновлений не нажимается и это должно происходить автоматически, если режим активирован
                    if (_mainSettings.Settings.ElectronicQueueMode)
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
                return new RelayCommand(async (o) =>
                {
                    await GetPerformance();
                });
            }
        }
        
        public RelayCommand CheckedGroup
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CheckGroup(); // Проверка выделенных групп
                    SortEmployees(); // Сортировка сотрудников                 
                });
            }
        }

        public RelayCommand CheckedOpenTasks
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    CheckOpenTasks(); // Проверка открытых заявок
                    SortEmployees(); // Сортировка сотрудников
                });
            }
        }

        public RelayCommand LeftClickStatusSelectButton
        {
            get
            {
                return new RelayCommand((o) =>
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
                return new RelayCommand((o) =>
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
                return new RelayCommand((o) =>
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
                return new RelayCommand((o) =>
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
                return  new RelayCommand((o) =>
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
                return new RelayCommand((o) =>
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
                return new RelayCommand((o) =>
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
                return new RelayCommand((o) =>
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

        public void SaveBackgroundPath(string path)
        {
            _mainSettings.Settings.PathToBackground = path;
            OnPropertyChanged(nameof(PathToBackground));
        }

        private async Task GetPerformance()
        {
            GettingTaskInRun = false;

            if (!timer.IsEnabled || (Employees?.Count == 0 || GroupEmployeeConnections.Count == 0 || Groups.Count == 0 || TaskTypes.Count == 0 || Priorities.Count == 0 || TaskStatuses.Count == 0))
                await CheckDictionaries();

            timer.Start();

            if (_mainSettings.Settings.ElectronicQueueMode)
            {
                SelectedDateTo = DateTime.Now;
                SelectedDateFrom = DateTime.Now;
            }

            if (Employees != null && Employees.Count != 0 && GroupEmployeeConnections.Count != 0 && Groups.Count != 0 && TaskTypes.Count != 0 && Priorities.Count != 0 && TaskStatuses.Count != 0)
            {
                if (await reportService.GetEmployeePerformance(Employees, SelectedDateFrom, SelectedDateTo))
                {
                    CheckGroup();
                    CheckOpenTasks();
                    SortEmployees();
                }
            }

            GettingTaskInRun = true;
        }

        private async Task CheckDictionaries()
        {
            var connections = await _request.GetRangeOfItems<EmployeeGroup>($"{mainServerLink}/employee/connections_with_group?startIndex=0");
            if (!UpdateCollection(connections, GroupEmployeeConnections)) return;

            var employees = await employeeService.GetEmployeesFromCloudApi();
            if (!UpdateCollection(employees, Employees)) return;

            var groups = (await _request.GetRangeOfItems<Group>($"{mainServerLink}/group/list", startIndex: 0))?.Cast<IOkdeskDictionary>().ToList();
            if (!UpdateOrRemoveDictionary(groups, Groups)) return;

            var priorities = (await _request.GetRangeOfItems<Priority>($"{mainServerLink}/issuePriority/list", startIndex: 0))?.Cast<IOkdeskDictionary>().ToList();
            if (!UpdateOrRemoveDictionary(priorities, Priorities)) return;

            var statuses = (await _request.GetRangeOfItems<Status>($"{mainServerLink}/issueStatus/list", startIndex: 0))?.Cast<IOkdeskDictionary>().ToList();
            if (!UpdateOrRemoveDictionary(statuses, TaskStatuses)) return;

            var taskTypes = (await _request.GetRangeOfItems<TaskType>($"{mainServerLink}/issueType/list", startIndex: 0))?.Cast<IOkdeskDictionary>().ToList();
            if (!UpdateOrRemoveDictionary(taskTypes, TaskTypes)) return;

            SaveDictionaries();
        }

        private void SortEmployees()
        {
            // Очистка списка активных сотрудников
            ActiveEmployees?.Clear();
            // Сортировка всех сотрудников по решённым заявкам по уменьшению
            if (Employees == null) return;

            // Сортировка по количеству решённых заявок
            Employees.Sort();
            foreach (var employee in Employees)
            {
                if (employee.IsSelected == false)
                    continue;

                // Если включен фильтр на запрет отображения сотрудников без решённых или открытых заявок / списанного времени,
                // то поменяет такого пользователя как не выделенным и пропускает в цикле
                if ((HideEmployeesWithoutSolvedIssues && employee.SolvedIssues == 0) || (HideEmployeesWithoutWrittenOffTime && employee.SpentedTime == 0) || (HideEmployeesWithoutOpenIssues && employee.OpenTasks == 0))
                {
                    employee.IsSelected = false;
                    continue;
                }

                // Если сотрудник активен, то есть имеет текущие заявки или списанное время, то добавить в активные
                if (employee.SolvedIssues != 0 || employee.OpenTasks != 0 || employee.SpentedTime != 0)
                    ActiveEmployees?.Add(employee);
            }
        }

        private void CheckGroup()
        {
            // Находит всех активных сотрудников
            // Выводит всех сотрудников принадлежащих к выбранной галочкой группой
            foreach (var employee in Employees)
            {
                // Проходит по всем чекбоксам групп
                for (var i = 0; i < Groups.Count; i++)
                {
                    // Находит в связях сотрудника с такой группой. Если сотрудник состоит в группе ...                                  
                    bool connection = GroupEmployeeConnections.Any(e => e.EmployeeId == employee.Id && e.GroupId == Groups[i].Id);
                    // и она выделена в чекбоксе, то:
                    if (connection == true && Groups[i].IsChecked == true)
                    {
                        // Если сотрудника отключили (убрав чекбокс с группы), а теперь включили назад,
                        // то вернуть "выделение"
                        if (employee.IsSelected == false)
                            employee.IsSelected = true;
                        break;
                    }

                    // Эта проверка нужна, чтобы цикл пробежался по всем группам сотрудника и если ни одна не выделена, то снимает выделение
                    if (i >= Groups.Count - 1)
                        employee.IsSelected = false;
                }
            }
        }

        private void CheckOpenTasks()
        {
            // HashSet для производительности, хранит все id
            HashSet<long>? selectedStatusIds = TaskStatuses?.Where(s => s.IsChecked).Select(s => s.Id).ToHashSet();
            HashSet<long>? selectedTypeIds = TaskTypes?.Where(t => t.IsChecked).Select(t => t.Id).ToHashSet();
            HashSet<long>? selectedPriorityIds = Priorities?.Where(p => p.IsChecked).Select(p => p.Id).ToHashSet();

            foreach (var employee in Employees)
            {
                employee.OpenTasks = 0;

                if (employee.Issues == null)
                    continue;

                employee.OpenTasks = employee.Issues.Count(i =>
                    i.StatusId.HasValue && selectedStatusIds != null && selectedStatusIds.Contains(i.StatusId.Value) &&
                    i.TypeId.HasValue && selectedTypeIds != null && selectedTypeIds.Contains(i.TypeId.Value) &&
                    i.PriorityId.HasValue && selectedPriorityIds != null && selectedPriorityIds.Contains(i.PriorityId.Value));
            }
        }

        private void SaveDictionaries()
        {
            _entitiesCache.Settings.Groups = Groups.Cast<Group>().ToList();
            _entitiesCache.Settings.Priorities = Priorities.Cast<Priority>().ToList();
            _entitiesCache.Settings.Statuses = TaskStatuses.Cast<Status>().ToList();
            _entitiesCache.Settings.Types = TaskTypes.Cast<TaskType>().ToList();
        }

        private bool UpdateCollection<T>(IList<T>? collectionFromAPI, IList<T> collectionFromCache)
        {
            if (collectionFromAPI == null) return false;
            try
            {
                foreach (var item in collectionFromAPI)
                {
                    collectionFromCache.Add(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in method {MethodName}", nameof(UpdateCollection));
                return false;
            }
        }

        private bool UpdateOrRemoveDictionary(IList<IOkdeskDictionary>? collectionFromAPI, ObservableCollection<IOkdeskDictionary> collectionInCache)
        {
            if (collectionFromAPI == null) return false;

            try
            {
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

                return true;
            }
            catch (Exception ex) { _logger.LogError(ex, "Error in method {MethodName}", nameof(UpdateOrRemoveDictionary)); return false; }
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is not ObservableCollection<IOkdeskDictionary> collection) return;

            if (e.NewItems != null)
            {
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += ItemPropertyChanged;

                foreach (var item in e.NewItems.OfType<IOkdeskDictionary>())
                {
                    var cacheList = GetCachedCollection(item);
                    if (cacheList != null && !cacheList.Any(c => c.Id == item.Id))
                    {
                        // Activator динамически создаёт экземпляры классов во время выполнения, это нужно для полиморфизма, чтобы не плодить код
                        var newItem = Activator.CreateInstance(item.GetType(), item) as IOkdeskDictionary;
                        cacheList.Add(newItem ?? item);
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= ItemPropertyChanged;

                foreach (var item in e.OldItems.OfType<IOkdeskDictionary>())
                {
                    var cacheList = GetCachedCollection(item);
                    var cacheItem = cacheList?.FirstOrDefault(c => c.Id == item.Id);
                    if (cacheItem != null)
                        cacheList?.Remove(cacheItem);
                }
            }
        }

        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is IOkdeskDictionary item)
            {
                var cachedCollection = GetCachedCollection(item);
                cachedCollection?.FirstOrDefault(p => p.Id == item.Id)?.Update(item);
            }
        }

        private IList<IOkdeskDictionary>? GetCachedCollection(IOkdeskDictionary item)
        {
            return item switch
            {
                Status => _entitiesCache.Settings.Statuses as IList<IOkdeskDictionary>,
                Priority => _entitiesCache.Settings.Priorities as IList<IOkdeskDictionary>,
                TaskType => _entitiesCache.Settings.Types as IList<IOkdeskDictionary>,
                _ => null
            };
        }

        #endregion
    }
}