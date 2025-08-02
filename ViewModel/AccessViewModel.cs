using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AqbaApp.Model.OkdeskEntities;
using System;
using System.Windows;
using System.IO;
using AqbaApp.Core;
using AqbaApp.Model.Client;
using Microsoft.Extensions.Logging;
using AqbaApp.Service.OkdeskEntity;
using AqbaApp.Service.Client;
using AqbaApp.Core.Api;
using System.Net.Http;
using AqbaApp.Interfaces.Service;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.Generic;
using AqbaApp.Interfaces.Api;

namespace AqbaApp.ViewModel
{
    public class AccessViewModel : NotifyProperty
    {
        #region [ Variables ]

        private readonly SettingService<MainSettings> _mainSettings;
        private readonly Immutable _immutable;
        private readonly ILogger<AccessViewModel> _logger;
        private readonly GetItemService _getItemService;
        private readonly IRequestService _requestService;
        private readonly EquipmentService _equipmentService;
        private readonly CompanyCategoryService _categoryService;
        private readonly CompanyService _companyService;
        private readonly MaintenanceEntityService _maintenanceService;
        private readonly HashSet<int> _filteredCompanyIds;
        private readonly HashSet<int> _objectCompanyIds;

        private readonly string baseApiLink;
        private readonly char[] separator;
        private string _clientSearchText;
        private string _maintenanceEntitySearchText;
        private string anydeskBtnVisibility;
        private string iikoOfficeBtnVisibility;
        private string iikoChainBtnVisibility;
        private string ammyAdminBtnVisibility;
        private string assistantBtnVisibility;
        private string rustdeskBtnVisibility;
        private bool gettingEntities;
        private Company? _selectedCompany;
        private MaintenanceEntity? _selectedMaintenanceEntity;
        private Equipment? currentEquipment;

        #endregion

        public AccessViewModel(SettingService<MainSettings> mainSettings, Immutable immutable, IHttpClientFactory client, INavigationService navigate, ILoggerFactory logger)
        {
            _mainSettings = mainSettings;
            _immutable = immutable;
            _getItemService = new(logger, client, navigate, mainSettings);
            _requestService = new RequestClient(client.CreateClient(), logger);
            _equipmentService = new(mainSettings, immutable, _getItemService);
            _categoryService = new(mainSettings, immutable, _getItemService);
            _companyService = new(mainSettings, immutable, _getItemService, _categoryService);
            _maintenanceService = new(mainSettings, immutable, _getItemService);
            _equipmentService = new(mainSettings, immutable, _getItemService);
            _logger = logger.CreateLogger<AccessViewModel>();
            Categories = [];
            Companies = [];
            MaintenanceEntities = [];
            Equipments = [];
            _filteredCompanyIds = [];
            _objectCompanyIds = [];
            FilteredCompanies = CollectionViewSource.GetDefaultView(Companies);
            FilteredCompanies.Filter = FilterCompanies;            
            FilteredObjects = CollectionViewSource.GetDefaultView(MaintenanceEntities);
            FilteredObjects.Filter = FilterObjects;

            baseApiLink = $"{_mainSettings.Settings.ServerAddress}/{_immutable.ApiMainEndpoint}";
            separator = ['/', '\\', ':'];
            _clientSearchText = string.Empty;
            _maintenanceEntitySearchText = string.Empty;
            anydeskBtnVisibility = "Collapsed";
            iikoOfficeBtnVisibility = "Collapsed";
            iikoChainBtnVisibility = "Collapsed";
            ammyAdminBtnVisibility = "Collapsed";
            assistantBtnVisibility = "Collapsed";
            rustdeskBtnVisibility = "Collapsed";
            gettingEntities = true;
        }

        #region [ Collections ]

        public ObservableCollection<Category>? Categories { get; set; }

        public ObservableCollection<Company>? Companies { get; set; }

        public ICollectionView FilteredCompanies { get; }
        
        public ICollectionView FilteredObjects { get; }

        public string CompanySearchText
        {
            get => _clientSearchText;
            set
            {
                if (_clientSearchText != value)
                {
                    _clientSearchText = value;
                    OnPropertyChanged(nameof(CompanySearchText));
                    RefreshFilters();              
                }
            }
        }

        public string MaintenanceEntitySearchText
        {
            get => _maintenanceEntitySearchText;
            set
            {
                if (_maintenanceEntitySearchText != value)
                {
                    _maintenanceEntitySearchText = value;
                    OnPropertyChanged(nameof(MaintenanceEntitySearchText));
                    RefreshFilters();
                }
            }
        }        

        public ObservableCollection<MaintenanceEntity>? MaintenanceEntities { get; set; }

        public ObservableCollection<Equipment>? Equipments { get; set; }

        public Company? SelectedCompany
        {
            get { return _selectedCompany; }
            set
            {
                _selectedCompany = value;
                OnPropertyChanged(nameof(SelectedCompany));
            }
        }

        public MaintenanceEntity? SelectedMaintenanceEntity
        {
            get { return _selectedMaintenanceEntity; }
            set
            {
                _selectedMaintenanceEntity = value;
                OnPropertyChanged(nameof(SelectedMaintenanceEntity));
            }
        }

        public Equipment? SelectedEquipment
        {
            get { return currentEquipment; }
            set
            {
                if (value == null)
                {
                    AnydeskBtnVisibility = "Collapsed";
                    IIKOOfficeBtnVisibility = "Collapsed";
                    IIKOChainBtnVisibility = "Collapsed";
                    AmmyAdminBtnVisibility = "Collapsed";
                    AssistantBtnVisibility = "Collapsed";
                    RustDesktBtnVisibility = "Collapsed";
                }
                else
                    currentEquipment = value;
                OnPropertyChanged(nameof(SelectedEquipment));
            }
        }

        public string CompanyResults => $"Отображено: {FilteredCompanies.Cast<object>().Count()} из {Companies?.Count} компаний";

        public string MaintenanceEntitiesResults => $"Отображено: {FilteredObjects.Cast<object>().Count()} из {MaintenanceEntities?.Count} объектов";

        public string AnydeskBtnVisibility
        {
            get { return anydeskBtnVisibility; }
            set
            {
                anydeskBtnVisibility = value;
                OnPropertyChanged(nameof(AnydeskBtnVisibility));
            }
        }

        public string IIKOOfficeBtnVisibility
        {
            get { return iikoOfficeBtnVisibility; }
            set
            {
                iikoOfficeBtnVisibility = value;
                OnPropertyChanged(nameof(IIKOOfficeBtnVisibility));
            }
        }

        public string IIKOChainBtnVisibility
        {
            get { return iikoChainBtnVisibility; }
            set
            {
                iikoChainBtnVisibility = value;
                OnPropertyChanged(nameof(IIKOChainBtnVisibility));
            }
        }

        public string AmmyAdminBtnVisibility
        {
            get { return ammyAdminBtnVisibility; }
            set
            {
                ammyAdminBtnVisibility = value;
                OnPropertyChanged(nameof(AmmyAdminBtnVisibility));
            }
        }

        public string AssistantBtnVisibility
        {
            get { return assistantBtnVisibility; }
            set
            {
                assistantBtnVisibility = value;
                OnPropertyChanged(nameof(AssistantBtnVisibility));
            }
        }

        public string RustDesktBtnVisibility
        {
            get { return rustdeskBtnVisibility; }
            set
            {
                rustdeskBtnVisibility = value;
                OnPropertyChanged(nameof(RustDesktBtnVisibility));
            }
        }

        public bool GettingEntities
        {
            get { return gettingEntities; }
            set
            {
                gettingEntities = value;
                OnPropertyChanged(nameof(GettingEntities));
            }
        }

        #endregion

        #region [ Commands ]
        public RelayCommand GetCompaniesCommand
        {
            get
            {
                return new RelayCommand(async (o) =>
                    {
                        GettingEntities = false;

                        var categories = await _categoryService.GetCategories();
                        if (categories == null || categories.Count == 0)
                        {
                            GettingEntities = true;
                            return;
                        }

                        Categories?.Clear();
                        foreach (var category in categories)
                            Categories?.Add(category);


                        var companies = await _companyService.GetCompaniesFromCloudApi();
                        if (companies == null || companies.Count == 0 || Companies == null)
                        {
                            GettingEntities = true;
                            return;
                        }

                        Companies.Clear();
                        foreach (var company in companies)
                            Companies.Add(company);

                        var maintenances = await _maintenanceService.GetMaintenanceEntitiesFromCloudApi();
                        if (maintenances == null || maintenances.Count == 0 || MaintenanceEntities == null)
                        {
                            GettingEntities = true;
                            return;
                        }

                        MaintenanceEntities.Clear();
                        foreach (var maintenance in maintenances)
                            MaintenanceEntities.Add(maintenance);

                        RefreshFilters();

                        GettingEntities = true;
                    });
            }
        }

        public RelayCommand AccessPageLoaded
        {
            get
            {
                return new RelayCommand((o) =>
                {

                });
            }
        }

        public RelayCommand SelectCompanyCommand
        {
            get
            {
                return new RelayCommand(async (selectedClient) =>
                {

                    if (selectedClient is Company currClient)
                    {
                        SelectedEquipment = null;
                        SelectedMaintenanceEntity = null;
                        SelectedCompany = currClient;

                        var equipments = await _equipmentService.GetEquipmentsByCompanyFromCloudApi(currClient.Id);
                        if (equipments != null && equipments.Count != 0)
                        {
                            Equipments?.Clear();
                            foreach (var equipment in equipments)
                                Equipments?.Add(equipment);
                        }
                    }
                });
            }
        }

        public RelayCommand SelectMaintenanceEntityCommand
        {
            get
            {
                return new RelayCommand(async (selectedObject) =>
                {
                    if (selectedObject is MaintenanceEntity currObject)
                    {
                        SelectedEquipment = null;
                        SelectedMaintenanceEntity = currObject;

                        var equipments = await _equipmentService.GetEquipmentsByMaintenanceEntityFromCloudApi(currObject.Id);
                        if (equipments != null && equipments.Count != 0)
                        {
                            Equipments?.Clear();
                            foreach (var equipment in equipments)
                                Equipments?.Add(equipment);
                        }
                    }
                });
            }
        }

        public RelayCommand SelectEquipmentCommand
        {
            get
            {
                return new RelayCommand((selectedEquipment) =>
                {
                    if (selectedEquipment is not Equipment equipment)
                        return;

                    SelectedEquipment = equipment;
                    HideStartBtnIcons();

                    CheckStartIconsBtnVisibility();
                });
            }
        }

        private void StartExternalTool(string exeName, string exePath, string paramCode, string argumentTemplate, string errorTitle, string exeNameForMessageBox)
        {
            if (SelectedEquipment == null) return;

            if (!CheckProgramFiles(exePath, exeNameForMessageBox, errorTitle))
                return;

            try
            {
                // Получает строку с доступом, например 123456789 / qwerty123
                string? rawValue = SelectedEquipment.Parameters?.Find(p => p?.Code == paramCode)?.Value?.Replace(" ", "");

                // Разделяет строку на id и пароль с помощью символа разделителя ( /, \, : )
                string[] idAndPass = rawValue?.Split(separator, StringSplitOptions.RemoveEmptyEntries) ?? [];

                if (idAndPass.Length == 0 || string.IsNullOrEmpty(idAndPass[0])) return;

                string id = idAndPass[0];
                string pass = idAndPass.Length > 1 ? idAndPass[1].Replace("%", "%%").Replace("&", "^&") : string.Empty;

                string arguments = string.Format(argumentTemplate, pass, exePath, id);

                ProcessStartInfo startInfo = new();
                startInfo.FileName = exeName;
                startInfo.Arguments = arguments;
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while open {ErrorTitle}.", errorTitle);
            }
        }

        public RelayCommand OpenAnydeskCommand => new(o =>
            StartExternalTool(
                exeName: "cmd.exe",
                exePath: _mainSettings.Settings.PathToAnydesk,
                paramCode: "AnyDesk",
                argumentTemplate: "/k echo {0} | \"{1}\" {2} --with-password && exit",
                errorTitle: "AnyDesk",
                exeNameForMessageBox: "AnyDesk.exe"));

        public RelayCommand OpenAmmyAdminCommand => new(o =>
            StartExternalTool(
                exeName: _mainSettings.Settings.PathToAmmyAdmin,
                exePath: _mainSettings.Settings.PathToAmmyAdmin,
                paramCode: "AA",
                argumentTemplate: " -connect {2} -password {0}",
                errorTitle: "Ammy admin",
                exeNameForMessageBox: "AA_3.exe"));

        public RelayCommand OpenAssistantCommand => new(o =>
            StartExternalTool(
                exeName: _mainSettings.Settings.PathToAssistant,
                exePath: _mainSettings.Settings.PathToAssistant,
                paramCode: "AC",
                argumentTemplate: " -CONNECT:{2}:{0}",
                errorTitle: "Ассистент РФ",
                exeNameForMessageBox: "Assistant.exe"));

        public RelayCommand OpenRustDeskCommand => new(o =>
            StartExternalTool(
                exeName: _mainSettings.Settings.PathToRustDesk,
                exePath: _mainSettings.Settings.PathToRustDesk,
                paramCode: "rustdesk",
                argumentTemplate: " --connect {2} --password {0}",
                errorTitle: "RustDesk",
                exeNameForMessageBox: "RustDesk.exe"));

        /*       public RelayCommand OpenAnydeskCommand
               {
                   get
                   {
                       return new RelayCommand((o) =>
                       {
                           if (SelectedEquipment == null) return;

                           try
                           {
                               string pathToAd = _mainSettings.Settings.PathToAnydesk;
                               if (string.IsNullOrEmpty(pathToAd))
                               {
                                   View.MessageBox.Show("Ошибка", "Укажите путь до AnyDesk.exe в настройках.", MessageBoxButton.OK);
                                   return;
                               }

                               if (!File.Exists(pathToAd))
                               {
                                   View.MessageBox.Show("Ошибка", "Не удалось найти файл AnyDesk.exe, проверьте путь в настройках.", MessageBoxButton.OK);
                                   return;
                               }

                               string? anydeskFull = SelectedEquipment.Parameters?.Find(equip => equip?.Code == "AnyDesk")?.Value.Replace(" ", "");
                               string[]? idAndPass = anydeskFull?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                               string id = string.Empty;
                               string pass = string.Empty;

                               if (idAndPass?.Length <= 0) return;

                               if (idAndPass?[0] != null) id = idAndPass[0];

                               if (string.IsNullOrEmpty(id)) return;

                               if (idAndPass?.Length > 1 && idAndPass[1] != null) pass = idAndPass[1].Replace("%", "%%").Replace("&", "^&");

                               var startInfo = new ProcessStartInfo()
                               {
                                   FileName = "cmd.exe",
                                   Arguments = $"/k echo {pass} | {"\"" + pathToAd + "\""} {id} --with-password && exit",
                                   UseShellExecute = false,
                                   CreateNoWindow = true
                               };
                               Process.Start(startInfo);
                           }
                           catch (Exception ex)
                           {
                               _logger.LogError(ex, "Error while open anydesk.");
                           }
                       });
                   }
               }

               public RelayCommand OpenAmmyAdminCommand
               {
                   get
                   {
                       return new RelayCommand((o) =>
                       {
                           if (SelectedEquipment == null) return;

                           try
                           {
                               string pathToAA = _mainSettings.Settings.PathToAmmyAdmin;
                               if (string.IsNullOrEmpty(pathToAA))
                               {
                                   View.MessageBox.Show("Ошибка", "Укажите путь до AA_3.exe в настройках", MessageBoxButton.OK);
                                   return;
                               }

                               if (!File.Exists(pathToAA))
                               {
                                   View.MessageBox.Show("Ошибка", "Не удалось найти файл AA_3.exe, проверьте путь в настройках", MessageBoxButton.OK);
                                   return;
                               }

                               string ammyadminFull = SelectedEquipment?.Parameters?.Find(equip => equip?.Code == "AA")?.Value?.Replace(" ", "");
                               string[] idAndPass = ammyadminFull?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                               string id = string.Empty;
                               string pass = string.Empty;

                               if (idAndPass?.Length <= 0) return;

                               if (idAndPass?[0] != null) id = idAndPass?[0];

                               if (string.IsNullOrEmpty(id)) return;

                               if (idAndPass?.Length > 1 && idAndPass?[1] != null) pass = idAndPass?[1];

                               var startInfo = new ProcessStartInfo()
                               {
                                   FileName = "\"" + pathToAA + "\"",
                                   Arguments = $" -connect {id} -password {pass}",
                                   UseShellExecute = false,
                                   CreateNoWindow = true
                               };
                               Process.Start(startInfo);
                           }
                           catch (Exception ex)
                           {
                               _logger.LogError(ex, "Error while open ammy admin.");
                           }
                       });
                   }
               }

               public RelayCommand OpenAssistantCommand
               {
                   get
                   {
                       return new RelayCommand((o) =>
                       {
                           if (SelectedEquipment == null) return;

                           try
                           {
                               string pathToAd = _mainSettings.Settings.PathToAssistant;
                               if (string.IsNullOrEmpty(pathToAd))
                               {
                                   View.MessageBox.Show("Ошибка", "Укажите путь до Assistant.exe в настройках", MessageBoxButton.OK);
                                   return;
                               }

                               if (!File.Exists(pathToAd))
                               {
                                   View.MessageBox.Show("Ошибка", "Не удаётся найти файл Assistant.exe, проверьте путь в настройках", MessageBoxButton.OK);
                                   return;
                               }

                               string assistantFull = SelectedEquipment?.Parameters?.Find(equip => equip?.Code == "AC")?.Value?.Replace(" ", "");
                               string[] idAndPass = assistantFull?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                               string id = string.Empty;
                               string pass = string.Empty;

                               if (idAndPass?.Length <= 0) return;

                               if (idAndPass?[0] != null) id = idAndPass?[0];

                               if (string.IsNullOrEmpty(id)) return;

                               if (idAndPass?.Length > 1 && idAndPass?[1] != null) pass = idAndPass?[1];

                               var startInfo = new ProcessStartInfo()
                               {
                                   FileName = "\"" + pathToAd + "\"",
                                   Arguments = $" -CONNECT:{id}:{pass}",
                                   UseShellExecute = false,
                                   CreateNoWindow = true
                               };
                               Process.Start(startInfo);
                           }
                           catch (Exception ex)
                           {
                               _logger.LogError(ex, "Error while open assistant rf.");
                           }
                       });
                   }
               }

               public RelayCommand OpenRustDeskCommand
               {
                   get
                   {
                       return new RelayCommand((o) =>
                       {
                           if (SelectedEquipment == null) return;

                           try
                           {
                               string pathToAd = _mainSettings.Settings.PathToRustDesk;
                               if (string.IsNullOrEmpty(pathToAd))
                               {
                                   View.MessageBox.Show("Ошибка", "Укажите путь до RustDesk.exe в настройках", MessageBoxButton.OK);
                                   return;
                               }

                               if (!File.Exists(pathToAd))
                               {
                                   View.MessageBox.Show("Ошибка", "Не удалось найти файл RustDesk.exe, проверьте путь в настройках", MessageBoxButton.OK);
                                   return;
                               }

                               string rustdeskFull = SelectedEquipment?.Parameters?.Find(equip => equip?.Code == "rustdesk")?.Value?.Replace(" ", "");
                               string[] idAndPass = rustdeskFull?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                               string id = string.Empty;
                               string pass = string.Empty;

                               if (idAndPass?.Length <= 0) return;

                               if (idAndPass?[0] != null) id = idAndPass?[0];

                               if (string.IsNullOrEmpty(id)) return;

                               if (idAndPass?.Length > 1 && idAndPass?[1] != null) pass = idAndPass?[1];

                               var startInfo = new ProcessStartInfo()
                               {
                                   FileName = "\"" + pathToAd + "\"",
                                   Arguments = $" --connect {id} --password {pass}",
                                   UseShellExecute = false,
                                   CreateNoWindow = true
                               };
                               Process.Start(startInfo);
                           }
                           catch (Exception ex)
                           {
                               _logger.LogError(ex, "Error while open RustDesk.");
                           }
                       });
                   }
               }*/

        public RelayCommand OpenIIKOOfficeCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedEquipment == null) return;

                    try
                    {
                        string pathToCLEARbat = _mainSettings.Settings.PathToCLEARbat;
                        if (!CheckProgramFiles(pathToCLEARbat, "CLEAR.bat.exe", "CLEAR.bat.exe"))
                            return;

                        string[] addressAndPort = [];
                        string[] loginAndPassword = [];

                        string? serverInfo = SelectedEquipment.Parameters.Find(equip => equip.Code == "srv_addr")?.Value.Replace(" ", "");
                        string? loginInfo = SelectedEquipment.Parameters.Find(equip => equip.Code == "0008")?.Value.Replace(" ", "");
                        
                        if (!string.IsNullOrWhiteSpace(serverInfo))
                            addressAndPort = serverInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        if (!string.IsNullOrEmpty(loginInfo))
                            loginAndPassword = loginInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        StartIIKO(pathToCLEARbat, loginAndPassword, addressAndPort);

                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while open iiko.");
                    }
                });
            }
        }

        public RelayCommand OpenIIKOChainCommand
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    if (SelectedEquipment == null) return;

                    try
                    {
                        string pathToCLEARbat = _mainSettings.Settings.PathToCLEARbat;
                        if (!CheckProgramFiles(pathToCLEARbat, "CLEAR.bat.exe", "CLEAR.bat.exe"))
                            return;

                        string[] addressAndPort = [];
                        string[] loginAndPassword = [];
                        string? serverInfo;

                        // Если открыт сервер RMS, в котором указан адрес чейна, то искать через код 0017
                        serverInfo = SelectedEquipment.Parameters.Find(equip => equip.Code == "0017")?.Value.Replace(" ", "");

                        // Если открыт чейн, то получать адрес через srv_addr
                        if (string.IsNullOrEmpty(serverInfo))
                            serverInfo = SelectedEquipment.Parameters.Find(equip => equip.Code == "srv_addr")?.Value.Replace(" ", "");

                        string? loginInfo = SelectedEquipment.Parameters.Find(equip => equip.Code == "0008")?.Value.Replace(" ", "");

                        if (!string.IsNullOrWhiteSpace(serverInfo))
                            addressAndPort = serverInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        if (!string.IsNullOrEmpty(loginInfo))
                            loginAndPassword = loginInfo.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                        StartIIKO(pathToCLEARbat, loginAndPassword, addressAndPort);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error while open iikoChain.");
                    }
                });
            }
        }

        private void StartIIKO(string pathToCLEARbat, string[] loginInfo, string[] serverInfo)
        {
            string address = string.Empty;
            string port = "443";
            string login = string.Empty;
            string password = string.Empty;

            if (serverInfo.Length == 0) return;

            if (serverInfo[0] != null) address = serverInfo[0];

            if (serverInfo.Length > 1 && serverInfo[1] != null) port = serverInfo[1];

            if (loginInfo != null && loginInfo.Length >= 0 && loginInfo[0] != null) login = loginInfo[0];

            if (loginInfo != null && loginInfo.Length > 1 && loginInfo[1] != null) password = loginInfo[1];

            if (string.IsNullOrEmpty(address)) return;

            Process.Start("\"" + pathToCLEARbat + "\"", $"iiko@{address}:{port}@{login}:{password}");
        }

        public RelayCommand UpdateCompanyCommand
        {
            get
            {
                return new RelayCommand(async (o) =>
                {
                    if (SelectedCompany == null) return;

                    // Отправка запрос в окдеск на сервер
                    await _requestService.SendPut(baseApiLink + $"/company/update_company_from_cloud_api?companyId={SelectedCompany.Id}", null, _mainSettings.Settings.Token?.AccessToken);

                    // Получение обновлённой компании с сервера
                    SelectedCompany = await _getItemService.GetItem<Company>(baseApiLink + $"/company?id={SelectedCompany.Id}");

                    if (SelectedCompany == null || Companies == null) return;

                    Company? companyFromList = Companies.Where(e => e.Id == SelectedCompany.Id).FirstOrDefault();

                    if (companyFromList == null) return;

                    companyFromList.Replace(SelectedCompany);
                });
            }
        }

        public RelayCommand UpdateMaintenanceEntityCommand
        {
            get
            {
                return new RelayCommand(async (o) =>
                {
                    if (SelectedMaintenanceEntity == null) return;

                    // Отправка запрос в окдеск на сервер
                    await _requestService.SendPut(baseApiLink + $"/maintenanceEntity/update_maintenance_entity_from_cloud_api?maintenanceEntityId={SelectedMaintenanceEntity.Id}", null, _mainSettings.Settings.Token?.AccessToken);

                    // Получение обновлённого объекта с сервера
                    SelectedMaintenanceEntity = await _getItemService.GetItem<MaintenanceEntity>(baseApiLink + $"/maintenanceEntity?id={SelectedMaintenanceEntity.Id}");

                    if (SelectedMaintenanceEntity == null || MaintenanceEntities == null) return;

                    MaintenanceEntity? maintenanceEntityFromList = MaintenanceEntities.Where(e => e.Id == SelectedMaintenanceEntity.Id).FirstOrDefault();

                    if (maintenanceEntityFromList == null) return;

                    maintenanceEntityFromList.Replace(SelectedMaintenanceEntity);
                });
            }
        }

        public RelayCommand UpdateEquipmentCommand
        {
            get
            {
                return new RelayCommand(async (o) =>
                {
                    if (SelectedEquipment == null) return;

                    HideStartBtnIcons();

                    // Отправка запрос в окдеск на сервер
                    await _requestService.SendPut(baseApiLink + $"/equipment?equipmentId={SelectedEquipment.Id}", null, _mainSettings.Settings.Token?.AccessToken);

                    // Получение обновлённого оборудования с сервера
                    SelectedEquipment = await _getItemService.GetItem<Equipment>(baseApiLink + $"/equipment?id={SelectedEquipment.Id}");

                    if (SelectedEquipment == null || Equipments == null) return;

                    Equipment? equipmentFromList = Equipments.Where(e => e.Id == SelectedEquipment.Id).FirstOrDefault();

                    if (equipmentFromList == null) return;

                    equipmentFromList.Replace(SelectedEquipment);

                    CheckStartIconsBtnVisibility();
                });
            }
        }
        #endregion

        #region [Methods]

        private void RefreshFilters()
        {
            FilteredCompanies.Refresh();
            UpdateFilteredCompanyIds();
            FilteredObjects.Refresh();
            UpdateObjectCompanyIds();

            OnPropertyChanged(nameof(CompanyResults));
            OnPropertyChanged(nameof(MaintenanceEntitiesResults));
        }

        private bool FilterCompanies(object obj)
        {
            if (obj is not Company company) return false;

            bool nameMatch = string.IsNullOrWhiteSpace(CompanySearchText)
                || company.Name.Contains(CompanySearchText, StringComparison.OrdinalIgnoreCase);

            bool relatedToFilteredObjects = string.IsNullOrWhiteSpace(MaintenanceEntitySearchText)
                || _objectCompanyIds.Contains(company.Id);

            return nameMatch && relatedToFilteredObjects;
        }

        private bool FilterObjects(object obj)
        {
            if (obj is not MaintenanceEntity entity) return false;

            // 1. Если нет поисковых строк — показываем вообще всё
            if (string.IsNullOrWhiteSpace(CompanySearchText) && string.IsNullOrWhiteSpace(MaintenanceEntitySearchText))
                return true;

            // 2. Если объект не привязан к компании — игнорим его (не отображаем), когда поисковая строка клиента не пустая
            if (entity?.CompanyId == null || !_filteredCompanyIds.Contains(entity.CompanyId))
                return false;

            // 3. Вывод только тех объектов, у которых CompanyId компаний, которые уже отфильтрованы
            bool companyMatch = _filteredCompanyIds.Contains(entity.CompanyId);

            // 4. Поиск объектов обслуживания, в имени которых содержится поисковая строка
            bool nameMatch = string.IsNullOrWhiteSpace(MaintenanceEntitySearchText)
                || entity.Name.Contains(MaintenanceEntitySearchText, StringComparison.OrdinalIgnoreCase);

            return companyMatch && nameMatch;
        }

        private void UpdateFilteredCompanyIds()
        {
            _filteredCompanyIds.Clear();

            foreach (Company company in FilteredCompanies)
                _filteredCompanyIds.Add(company.Id);
        }

        private void UpdateObjectCompanyIds()
        {
            _objectCompanyIds.Clear();

            foreach (MaintenanceEntity entity in FilteredObjects)
            {
                if (!_objectCompanyIds.Contains(entity.CompanyId))
                    _objectCompanyIds.Add(entity.CompanyId);
            }
        }



        private void HideStartBtnIcons()
        {
            AnydeskBtnVisibility = "Collapsed";
            IIKOOfficeBtnVisibility = "Collapsed";
            IIKOChainBtnVisibility = "Collapsed";
            AmmyAdminBtnVisibility = "Collapsed";
            AssistantBtnVisibility = "Collapsed";
            RustDesktBtnVisibility = "Collapsed";
        }

        private void CheckStartIconsBtnVisibility()
        {
            if (SelectedEquipment == null) return;

            if (SelectedEquipment.Parameters?.Find(equip => equip.Code == "AnyDesk") != null)
                AnydeskBtnVisibility = "Visible";
            else AnydeskBtnVisibility = "Collapsed";

            if (SelectedEquipment.Parameters?.Find(equip => equip.Code == "srv_addr") != null &&
            SelectedEquipment.Kind?.Code != "0010")
                IIKOOfficeBtnVisibility = "Visible";
            else IIKOOfficeBtnVisibility = "Collapsed";

            if (SelectedEquipment.Parameters?.Find(equip => equip.Code == "0017") != null ||
            SelectedEquipment.Kind?.Code == "0010")
                IIKOChainBtnVisibility = "Visible";
            else IIKOChainBtnVisibility = "Collapsed";

            if (SelectedEquipment.Parameters?.Find(equip => equip.Code == "AA") != null)
                AmmyAdminBtnVisibility = "Visible";
            else AmmyAdminBtnVisibility = "Collapsed";

            if (SelectedEquipment.Parameters?.Find(equip => equip.Code == "AC") != null)
                AssistantBtnVisibility = "Visible";
            else AssistantBtnVisibility = "Collapsed";

            if (SelectedEquipment.Parameters?.Find(equip => equip.Code == "rustdesk") != null)
                RustDesktBtnVisibility = "Visible";
            else RustDesktBtnVisibility = "Collapsed";
        }

        private bool CheckProgramFiles(string pathToFile, string exeNameForMessageBox, string titleName = "Ошибка")
        {
            if (string.IsNullOrEmpty(pathToFile))
            {
                View.MessageBox.Show(titleName, $"Укажите путь до {exeNameForMessageBox} в настройках", MessageBoxButton.OK);
                return false;
            }

            if (!File.Exists(pathToFile))
            {
                View.MessageBox.Show(titleName, $"Не удалось найти файл {exeNameForMessageBox}, проверьте путь в настройках", MessageBoxButton.OK);
                return false;
            }

            return true;
        }

        #endregion
    }
}
