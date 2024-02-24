using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using AqbaApp.Model.OkdeskEntities;
using AqbaApp.API;

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
        ObservableCollection<Category> categories;
        ObservableCollection<Company> companies;
        ObservableCollection<Company> filteredListOfCompanies;
        ObservableCollection<Company> tempListOfCompanies;
        ObservableCollection<MaintenanceEntity> objects;
        ObservableCollection<MaintenanceEntity> filteredListOfMaintenanceEntities;
        ObservableCollection<MaintenanceEntity> tempListOfMaintenanceEntities;
        ObservableCollection<Equipment> equipment;
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
                OnPropertyChanged("AnydeskBtnVisibility");
            }
        }

        public string IIKOOfficeBtnVisibility
        {
            get { return iikoOfficeBtnVisibility; }
            set
            {
                iikoOfficeBtnVisibility = value;
                OnPropertyChanged("IIKOOfficeBtnVisibility");
            }
        }

        public string IIKOChainBtnVisibility
        {
            get { return iikoChainBtnVisibility; }
            set
            {
                iikoChainBtnVisibility = value;
                OnPropertyChanged("IIKOChainBtnVisibility");
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
                        tempListOfCompanies = new ObservableCollection<Company>();
                        CompanyResults = $"Загружено {Companies.Count} компаний";

                        await Request.GetMaintenanceEntities(Objects);

                        FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects);
                        tempListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>();
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
                        Equipments?.Clear();
                        CurrentEquipment = null;

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
                            FilteredListOfCompanies = new ObservableCollection<Company>(tempListOfCompanies.Where(comp => comp.Name.ToLower().Contains(searchCompanyString)));
                        else
                            FilteredListOfCompanies = new ObservableCollection<Company>(Companies.Where(comp => comp.Name.ToLower().Contains(searchCompanyString)));

                        FilteredListOfMaintenanceEntities?.Clear();
                        tempListOfMaintenanceEntities?.Clear();

                        foreach (var item in FilteredListOfCompanies)
                        {
                            for (int i = 0; i < Objects.Count; i++)
                            {
                                if (item.Id == Objects[i].Company_Id)
                                    if (Objects[i].Name.Trim().ToLower().Contains(searchMaintenanceEntitiesString))
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
                            FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(tempListOfMaintenanceEntities.Where(cl => cl.Name.ToLower().Contains(searchMaintenanceEntitiesString)));
                        else
                            FilteredListOfMaintenanceEntities = new ObservableCollection<MaintenanceEntity>(Objects.Where(cl => cl.Name.ToLower().Contains(searchMaintenanceEntitiesString)));

                        FilteredListOfCompanies.Clear();
                        tempListOfCompanies.Clear();

                        foreach (var item in FilteredListOfMaintenanceEntities)
                        {
                            for (int i = 0; i < Companies.Count; i++)
                            {
                                if (Companies[i].Id == item.Company_Id && !FilteredListOfCompanies.Contains(Companies[i]))
                                    if (Companies[i].Name.Trim().ToLower().Contains(searchCompanyString))
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
                return selectedEquipment ??= new RelayCommand(async(selectedEquipment) =>
                {
                    if (selectedEquipment is Equipment equipment)
                    {
                        CurrentEquipment = equipment;
                        HideIcons();
                        Equipment updatedEquipment = await Request.GetEquipment(equipment.Id);

                        if (updatedEquipment != null)
                        {
                            CurrentEquipment = updatedEquipment;
                            var equip = Equipments.Where(e => e.Id == updatedEquipment.Id).First();                            
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
                    if (CurrentEquipment != null)
                    {
                        string pathToAd = @"""C:\Program Files (x86)\AnyDesk\Anydesk.exe""";
                        
                        string anydeskFull = CurrentEquipment.Parameters.Find(equip => equip.Code == "AnyDesk").Value.Replace(" ", "");
                        string[] idAndPass = anydeskFull.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string id = string.Empty;
                        string pass = string.Empty;

                        if (idAndPass.Length <= 0)
                            return;

                        if (idAndPass[0] != null)
                            id = idAndPass[0];

                        if (idAndPass.Length > 1 && idAndPass[1] != null)
                            pass = idAndPass[1];

                        if (!string.IsNullOrEmpty(id))
                        {
                            var startInfo = new ProcessStartInfo()
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/k echo {pass} | {pathToAd} {id} --with-password && exit",
                                UseShellExecute = false,
                                CreateNoWindow = true
                            };
                            Process.Start(startInfo);
                        }
                    }
                });
            }
        }

        public RelayCommand OpenIIKO
        {
            get
            {
                return openOffice ??= new RelayCommand((o) =>
                {

                    if (CurrentEquipment != null)
                    {
                        string serverInfo = CurrentEquipment.Parameters.Find(equip => equip.Code == "srv_addr").Value.Replace(" ", "");
                        string loginInfo = CurrentEquipment.Parameters.Find(equip => equip.Code == "0008").Value.Replace(" ", "");
                        string[] addressAndPort = serverInfo.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] loginAndPassword = loginInfo.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        StartIIKO(loginAndPassword, addressAndPort);
                    }
                });
            }
        }

        public RelayCommand OpenChain
        {
            get
            {
                return openChain ??= new RelayCommand((o) =>
                {
                    if (CurrentEquipment != null)
                    {
                        string serverInfo = CurrentEquipment.Parameters.Find(equip => equip.Code == "0017").Value.Replace(" ", ""); // 0017 код атрибута iikoChain
                        string loginInfo = CurrentEquipment.Parameters.Find(equip => equip.Code == "0008").Value.Replace(" ", "");
                        string[] addressAndPort = serverInfo.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        string[] loginAndPassword = loginInfo.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
                        StartIIKO(loginAndPassword, addressAndPort);
                    }
                });
            }
        }

        #endregion

        #region [Methods]

        static void StartIIKO(string[] loginInfo, string[] serverInfo)
        {
            string pathToCLEARbat = @"""C:\iiko_Distr\CLEAR.bat.exe""";
            string address = string.Empty;
            string port = "443";
            string login = string.Empty;
            string password = string.Empty;

            if (serverInfo.Length <= 0)
                return;

            if (serverInfo[0] != null)
                address = serverInfo?[0];

            if (serverInfo.Length > 1 && serverInfo[1] != null)
                port = serverInfo[1];

            if (loginInfo.Length >= 0 && loginInfo[0] != null)
                login = loginInfo?[0];

            if (loginInfo.Length > 1 && loginInfo[1] != null)
                password = loginInfo?[1];


            if (!string.IsNullOrEmpty(address))
            {
                string args = $"iiko@{address}:{port}@{login}:{password}";
                Process.Start($"{pathToCLEARbat}", args);
            }
        }

        void HideIcons()
        {
            AnydeskBtnVisibility = "Collapsed";
            IIKOOfficeBtnVisibility = "Collapsed";
            IIKOChainBtnVisibility = "Collapsed";
        }

        #endregion
    }
}
