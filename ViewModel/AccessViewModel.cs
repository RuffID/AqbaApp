using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AqbaApp.Model.OkdeskEntities;
using AqbaApp.API;
using System;
using System.Windows;
using System.IO;

namespace AqbaApp.ViewModel
{
    public class AccessViewModel : ViewModelBase
    {
        public AccessViewModel() 
        {
            Categories = [];
            Companies = [];
            Objects = [];
            Equipments = [];
            FilteredListOfCompanies = new ObservableCollection<Company>(Companies);
            FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects);
            tempListOfCompanies = [];
            tempListOfMaintenanceEntities = [];
        }

        #region [ Variables ]

        private static readonly char[] separator = ['/', '\\', ':'];
        string searchCompanyString = "";
        string searchMaintenanceEntitiesString = "";
        string companyResult = "";
        string maintenanceEntitiesResult = "";
        string anydeskBtnVisibility = "Collapsed";
        string iikoOfficeBtnVisibility = "Collapsed";
        string iikoChainBtnVisibility = "Collapsed";
        string ammyAdminBtnVisibility = "Collapsed";
        string assistantBtnVisibility = "Collapsed";
        ObservableCollection<Category> categories;
        ObservableCollection<Company> companies;
        ObservableCollection<Company> filteredListOfCompanies;
        ObservableCollection<Company> tempListOfCompanies;
        ObservableCollection<MaintenanceEntity> objects;
        ObservableCollection<MaintenanceEntity> filteredListOfMaintenanceEntities;
        ObservableCollection<MaintenanceEntity> tempListOfMaintenanceEntities;
        ObservableCollection<Equipment> equipment;
        Company currentCompany;
        MaintenanceEntity currentMaintenanceEntity;
        Equipment currentEquipment;
        RelayCommand getClients;
        RelayCommand selectedClient;
        RelayCommand selectedObject;
        RelayCommand selectedEquipment;
        RelayCommand clientSearch;
        RelayCommand objectSearch;
        RelayCommand accessPageLoaded;
        RelayCommand openAnydesk;
        RelayCommand openOffice;
        RelayCommand openChain;
        RelayCommand openAmmyAdmin;
        RelayCommand openAssistant;
        RelayCommand updateEquipment;
        RelayCommand updateCompany;
        RelayCommand updateMaintenanceEntity;

        #endregion

        #region [ Collections ]

        public ObservableCollection<Category> Categories 
        { 
          get { return categories; }
          set { categories = value; OnPropertyChanged(nameof(Categories)); } 
        }

        public ObservableCollection<Company> Companies
        {
            get { return companies; }
            set { companies = value; OnPropertyChanged(nameof(Companies)); }
        }

        public ObservableCollection<Company> FilteredListOfCompanies
        {
            get { return filteredListOfCompanies; }
            set { filteredListOfCompanies = value; OnPropertyChanged(nameof(FilteredListOfCompanies)); }
        }

        public ObservableCollection<MaintenanceEntity> FilteredListOfMaintenanceEntities
        {
            get { return filteredListOfMaintenanceEntities; }
            set { filteredListOfMaintenanceEntities = value; OnPropertyChanged(nameof(FilteredListOfMaintenanceEntities)); }
        }

        public ObservableCollection<MaintenanceEntity> Objects
        {
            get { return objects; }
            set { objects = value; OnPropertyChanged(nameof(Objects)); }
        }

        public ObservableCollection<Equipment> Equipments
        {
            get { return equipment; }
            set { equipment = value; OnPropertyChanged(nameof(Equipments)); }
        }

        public Company CurrentCompany
        {
            get { return currentCompany; }
            set
            {
                currentCompany = value;
                OnPropertyChanged(nameof(CurrentCompany));
            }
        }

        public MaintenanceEntity CurrentMaintenanceEntity
        {
            get { return currentMaintenanceEntity; }
            set
            {
                currentMaintenanceEntity = value;
                OnPropertyChanged(nameof(CurrentMaintenanceEntity));
            }
        }

        public Equipment CurrentEquipment
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
                }
                currentEquipment = value;
                OnPropertyChanged(nameof(CurrentEquipment));
            }
        }

        public string CompanyResults
        {
            get { return companyResult; }
            set
            {
                companyResult = value;
                OnPropertyChanged("CompanyResults");
            }
        }

        public string MaintenanceEntitiesResults
        {
            get { return maintenanceEntitiesResult; }
            set
            {
                maintenanceEntitiesResult = value;
                OnPropertyChanged("MaintenanceEntitiesResults");
            }
        }

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


        #endregion

        #region [ Commands ]
        public RelayCommand GetClients
        {
            get
            {
                return getClients ??= new RelayCommand(async (o) =>
                //TODO сделай сортировку клиентов по алфавиту
                    {
                        if (!await Request.GetCategories(Categories))
                            return;
                        await Request.GetCompanies(Companies, Categories);

                        FilteredListOfCompanies = new ObservableCollection<Company>(Companies);
                        tempListOfCompanies = [];
                        CompanyResults = $"Загружено {Companies.Count} компаний";

                        await Request.GetMaintenanceEntities(Objects);

                        FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects);
                        tempListOfMaintenanceEntities = [];
                        MaintenanceEntitiesResults = $"Загружено {Objects.Count} объектов";
                    });
            }
        }

        public RelayCommand SelectedClient
        {
            get
            {
                return selectedClient ??= new RelayCommand(async (selectedClient) =>
                {

                    if (selectedClient is Company currClient)
                    {
                        CurrentEquipment = null;
                        CurrentMaintenanceEntity = null;
                        CurrentCompany = currClient;

                        FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects.Where(obj => obj.Company_Id == currClient.Id));

                        //TODO сделать поиск объектов по companyId
                        await Request.GetEquipmentsByCompany(Equipments, currClient.Id);
                    }
                });
            }
        }

        public RelayCommand SelectedObject
        {
            get
            {
                return selectedObject ??= new RelayCommand(async (selectedObject) =>
                {
                    if (selectedObject is MaintenanceEntity currObject)
                    {          
                        CurrentEquipment = null;
                        CurrentMaintenanceEntity = currObject;

                        await Request.GetEquipmentsByMaintenanceEntity(Equipments, currObject.Id);                        
                    }
                });
            }
        }

        public RelayCommand ClientSearch
        {
            get
            {
                return clientSearch ??= new RelayCommand((text) =>
                {
                    CompanyResults = "Найдено 0 из 0";
                    if (Companies?.Count > 0)
                    {
                        searchCompanyString = text.ToString().Trim().ToLower();

                        // Поиск и заполнение списка компаний по поисковой строке
                        if (searchMaintenanceEntitiesString != "")
                            FilteredListOfCompanies = new ObservableCollection<Company>(tempListOfCompanies.Where(comp => comp.Name.Contains(searchCompanyString, StringComparison.CurrentCultureIgnoreCase)));
                        else
                            FilteredListOfCompanies = new ObservableCollection<Company>(Companies.Where(comp => comp.Name.Contains(searchCompanyString, StringComparison.CurrentCultureIgnoreCase)));

                        FilteredListOfMaintenanceEntities?.Clear();
                        tempListOfMaintenanceEntities?.Clear();

                        foreach (var item in FilteredListOfCompanies)
                        {
                            for (int i = 0; i < Objects.Count; i++)
                            {
                                if (item.Id == Objects[i].Company_Id)
                                    if (Objects[i].Name.Trim().Contains(searchMaintenanceEntitiesString, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        FilteredListOfMaintenanceEntities.Add(Objects[i]);
                                        tempListOfMaintenanceEntities.Add(Objects[i]);
                                    }
                            }
                        }
                        if (searchCompanyString == "" && searchMaintenanceEntitiesString == "")
                            FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects);

                        CompanyResults = $"Найдено {FilteredListOfCompanies.Count} из {Companies.Count}";
                        MaintenanceEntitiesResults = $"Найдено {FilteredListOfMaintenanceEntities.Count} из {Objects.Count}";
                    }
                });
            }
        }

        public RelayCommand ObjectSearch
        {
            get
            {
                return objectSearch ??= new RelayCommand((text) =>
                {
                    MaintenanceEntitiesResults = "Найдено 0 из 0";
                    if (Objects?.Count > 0)
                    {
                        searchMaintenanceEntitiesString = text.ToString().Trim().ToLower();

                        // Поиск и заполнение списка объектов по поисковой строке
                        if (searchCompanyString != "")
                            FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(tempListOfMaintenanceEntities.Where(cl => cl.Name.Contains(searchMaintenanceEntitiesString, StringComparison.CurrentCultureIgnoreCase)));
                        else
                            FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects.Where(cl => cl.Name.Contains(searchMaintenanceEntitiesString, StringComparison.CurrentCultureIgnoreCase)));

                        FilteredListOfCompanies.Clear();
                        tempListOfCompanies.Clear();

                        foreach (var item in FilteredListOfMaintenanceEntities)
                        {
                            for (int i = 0; i < Companies.Count; i++)
                            {
                                if (Companies[i].Id == item.Company_Id && !FilteredListOfCompanies.Contains(Companies[i]))
                                    if (Companies[i].Name.Trim().Contains(searchCompanyString, StringComparison.CurrentCultureIgnoreCase))
                                    {
                                        FilteredListOfCompanies.Add(Companies[i]);
                                        tempListOfCompanies.Add(Companies[i]);
                                    }
                            }
                        }
                        if (searchMaintenanceEntitiesString == "" && searchCompanyString == "")
                            FilteredListOfCompanies = new ObservableCollection<Company>(Companies);

                        MaintenanceEntitiesResults = $"Найдено {FilteredListOfMaintenanceEntities.Count} из {Objects.Count}";
                        CompanyResults = $"Найдено {FilteredListOfCompanies.Count} из {Companies.Count}";
                    }
                });
            }
        }

        public RelayCommand SelectedEquipment
        {
            get
            {
                return selectedEquipment ??= new RelayCommand((selectedEquipment) =>
                {
                    if (selectedEquipment is Equipment equipment)
                    {
                        CurrentEquipment = equipment;
                        HideIcons();

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "AnyDesk") != null)
                            AnydeskBtnVisibility = "Visible";
                        else AnydeskBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "srv_addr") != null &&
                        CurrentEquipment.Equipment_kind?.Code != "0010")
                            IIKOOfficeBtnVisibility = "Visible";
                        else IIKOOfficeBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "0017") != null ||
                        CurrentEquipment.Equipment_kind?.Code == "0010")
                            IIKOChainBtnVisibility = "Visible";
                        else IIKOChainBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "AA") != null)
                            AmmyAdminBtnVisibility = "Visible";
                        else AmmyAdminBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "AC") != null)
                            AssistantBtnVisibility = "Visible";
                        else AssistantBtnVisibility = "Collapsed";
                    }
                });
            }
        }

        public RelayCommand UpdateEquipment
        {
            get
            {
                return updateEquipment ??= new RelayCommand( async (o) =>
                {
                    if (CurrentEquipment != null)
                    {
                        HideIcons();
                        Equipment updatedEquipment = await Request.GetEquipment(CurrentEquipment.Id);

                        if (updatedEquipment != null)
                        {
                            CurrentEquipment = updatedEquipment;
                            var equip = Equipments.Where(e => e.Id == updatedEquipment.Id).FirstOrDefault();
                            if (equip != null)
                            {
                                int index = Equipments.IndexOf(equip);
                                if (index >= 0)
                                    Equipments[index].Replace(updatedEquipment);
                            }
                        }

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "AnyDesk") != null)
                            AnydeskBtnVisibility = "Visible";
                        else AnydeskBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "srv_addr") != null &&
                        CurrentEquipment.Equipment_kind?.Code != "0010")
                            IIKOOfficeBtnVisibility = "Visible";
                        else IIKOOfficeBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "0017") != null ||
                        CurrentEquipment.Equipment_kind?.Code == "0010")
                            IIKOChainBtnVisibility = "Visible";
                        else IIKOChainBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "AA") != null)
                            AmmyAdminBtnVisibility = "Visible";
                        else AmmyAdminBtnVisibility = "Collapsed";

                        if (CurrentEquipment.Parameters?.Find(equip => equip.Code == "AC") != null)
                            AssistantBtnVisibility = "Visible";
                        else AssistantBtnVisibility = "Collapsed";
                    }
                });
            }
        }

        public RelayCommand AccessPageLoaded
        {
            get
            {
                return accessPageLoaded ??= new RelayCommand((o) =>
                {
                    //await IIKOUserMain.Login();
                });
            }
        }

        public RelayCommand OpenAnydesk
        {
            get
            {
                return openAnydesk ??= new RelayCommand((o) =>
                {
                    if (CurrentEquipment == null) return;

                    try
                    {
                        string pathToAd = Config.Settings.PathToAnydesk;
                        if (string.IsNullOrEmpty(pathToAd))
                        {
                            View.MessageBox.Show("Ошибка", "Укажите путь до AnyDesk.exe в настройках", MessageBoxButton.OK);
                            return;
                        }

                        if (!File.Exists(pathToAd))
                        {
                            View.MessageBox.Show("Ошибка", "Не удалось найти файл AnyDesk.exe, проверьте путь в настройках", MessageBoxButton.OK);
                            return;
                        }

                        string anydeskFull = CurrentEquipment?.Parameters?.Find(equip => equip?.Code == "AnyDesk")?.Value?.Replace(" ", "");
                        string[] idAndPass = anydeskFull?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string id = string.Empty;
                        string pass = string.Empty;

                        if (idAndPass?.Length <= 0) return;

                        if (idAndPass?[0] != null) id = idAndPass?[0];

                        if (string.IsNullOrEmpty(id)) return;

                        if (idAndPass?.Length > 1 && idAndPass?[1] != null) pass = idAndPass?[1]?.Replace("%", "%%")?.Replace("&", "^&");

                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/k echo {pass} | {"\"" + pathToAd + "\""} {id} --with-password && exit",
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        Process.Start(startInfo);
                    }
                    catch (Exception ex) { WriteLog.Error(ex.ToString()); }
                });
            }
        }

        public RelayCommand OpenAmmyAdmin
        {
            get
            {
                return openAmmyAdmin ??= new RelayCommand((o) =>
                {
                    if (CurrentEquipment == null) return;

                    try
                    {
                        string pathToAA = Config.Settings.PathToAmmyAdmin;
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

                        string ammyadminFull = CurrentEquipment?.Parameters?.Find(equip => equip?.Code == "AA")?.Value?.Replace(" ", "");
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
                    catch (Exception ex) { WriteLog.Error(ex.ToString()); }
                });
            }
        }

        public RelayCommand OpenAssistant
        {
            get
            {
                return openAssistant ??= new RelayCommand((o) =>
                {
                    if (CurrentEquipment == null) return;

                    try
                    {
                        string pathToAd = Config.Settings.PathToAssistant;
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

                        string assistantFull = CurrentEquipment?.Parameters?.Find(equip => equip?.Code == "AC")?.Value?.Replace(" ", "");
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
                    catch (Exception ex) { WriteLog.Error(ex.ToString()); }
                });
            }
        }

        public RelayCommand OpenIIKOOffice
        {
            get
            {
                return openOffice ??= new RelayCommand((o) =>
                {
                    if (CurrentEquipment == null) return;

                    try
                    {
                        string pathToCLEARbat = Config.Settings.PathToCLEARbat;
                        if (string.IsNullOrEmpty(pathToCLEARbat))
                        {
                            View.MessageBox.Show("Ошибка", "Укажите путь до CLEAR.bat.exe в настройках", MessageBoxButton.OK);
                            return;
                        }    

                        if (!File.Exists(pathToCLEARbat))
                        {
                            View.MessageBox.Show("Ошибка", "Не удалось найти файл CLEAR.bat.exe, проверьте путь в настройках", MessageBoxButton.OK);
                            return;
                        }

                        string serverInfo = CurrentEquipment?.Parameters?.Find(equip => equip?.Code == "srv_addr")?.Value?.Replace(" ", "");
                        string loginInfo = CurrentEquipment?.Parameters?.Find(equip => equip?.Code == "0008")?.Value?.Replace(" ", "");
                        string[] addressAndPort = serverInfo?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] loginAndPassword = loginInfo?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        StartIIKO(pathToCLEARbat, loginAndPassword, addressAndPort);

                    }
                    catch (Exception e) { WriteLog.Error(e.ToString()); }
                });
            }
        }

        public RelayCommand OpenIIKOChain
        {
            get
            {
                return openChain ??= new RelayCommand((o) =>
                {
                    if (CurrentEquipment == null) return;

                    try
                    {
                        string pathToCLEARbat = Config.Settings.PathToCLEARbat;
                        if (string.IsNullOrEmpty(pathToCLEARbat))
                        {
                            View.MessageBox.Show("Ошибка", "Укажите путь до CLEAR.bat.exe в настройках", MessageBoxButton.OK);
                            return;
                        }

                        if (!File.Exists(pathToCLEARbat))
                        {
                            View.MessageBox.Show("Ошибка", "Не удалось найти файл CLEAR.bat.exe, проверьте путь в настройках", MessageBoxButton.OK);
                            return;
                        }

                        string serverInfo = CurrentEquipment?.Parameters?.Find(equip => equip?.Code == "srv_addr")?.Value?.Replace(" ", ""); // 0017 код атрибута iikoChain
                        string loginInfo = CurrentEquipment?.Parameters?.Find(equip => equip.Code == "0008")?.Value?.Replace(" ", "");
                        string[] addressAndPort = serverInfo?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] loginAndPassword = loginInfo?.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        StartIIKO(pathToCLEARbat, loginAndPassword, addressAndPort);
                    }
                    catch (Exception e) { WriteLog.Error(e.ToString()); }
                });
            }
        }

        public RelayCommand UpdateCompany
        {
            get
            {
                return updateCompany ??= new RelayCommand(async(o) =>
                {
                    if (CurrentCompany != null)
                    {
                        Company updatedCompany = await Request.GetCompany(CurrentCompany.Id);

                        if (updatedCompany != null)
                        {
                            CurrentCompany = updatedCompany;

                            var comp = Companies.Where(e => e.Id == updatedCompany.Id).FirstOrDefault();
                            if (comp != null)
                            {
                                int index = Companies.IndexOf(comp);
                                if (index >= 0)
                                    Companies[index].Replace(updatedCompany);
                            }
                        }
                    }
                });
            }
        }

        public RelayCommand UpdateMaintenanceEntity
        {
            get
            {
                return updateMaintenanceEntity ??= new RelayCommand(async (o) =>
                {
                    if (CurrentMaintenanceEntity != null)
                    {
                        MaintenanceEntity updatedME = await Request.GetMaintenanceEntity(CurrentMaintenanceEntity.Id);

                        if (updatedME != null)
                        {
                            CurrentMaintenanceEntity = updatedME;

                            var me = Objects.Where(e => e.Id == updatedME.Id).FirstOrDefault();
                            if (me != null)
                            {
                                int index = Objects.IndexOf(me);
                                if (index >= 0)
                                    Objects[index].Replace(updatedME);
                            }
                        }
                    }
                });
            }
        }

        #endregion

        #region [Methods]

        static void StartIIKO(string pathToCLEARbat, string[] loginInfo, string[] serverInfo)
        {
            string address = string.Empty;
            string port = "443";
            string login = string.Empty;
            string password = string.Empty;

            if (serverInfo.Length <= 0) return;

            if (serverInfo[0] != null) address = serverInfo?[0];

            if (serverInfo?.Length > 1 && serverInfo?[1] != null) port = serverInfo[1];

            if (loginInfo?.Length >= 0 && loginInfo?[0] != null) login = loginInfo?[0];

            if (loginInfo?.Length > 1 && loginInfo?[1] != null) password = loginInfo?[1];

            if (string.IsNullOrEmpty(address)) return;

            Process.Start("\"" + pathToCLEARbat + "\"", $"iiko@{address}:{port}@{login}:{password}");
        }

        void HideIcons()
        {
            AnydeskBtnVisibility = "Collapsed";
            IIKOOfficeBtnVisibility = "Collapsed";
            IIKOChainBtnVisibility = "Collapsed";
            AmmyAdminBtnVisibility = "Collapsed";
            AssistantBtnVisibility = "Collapsed";
        }

        #endregion
    }
}
