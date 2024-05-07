namespace AqbaApp.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel() 
        {
            PathToCLEARbat = Config.Settings.PathToCLEARbat;
            PathToAnydesk = Config.Settings.PathToAnydesk;
            PathToAmmyAdmin = Config.Settings.PathToAmmyAdmin;
            PathToAssistant = Config.Settings.PathToAssistant;
            PathToRustDesk = Config.Settings.PathToRustDesk;

            if (string.IsNullOrEmpty(PathToCLEARbat))
            {
                PathToCLEARbat = "C:\\iiko_Distr\\CLEAR.bat.exe";
                Config.Settings.PathToCLEARbat = PathToCLEARbat;
            }

            if (string.IsNullOrEmpty(PathToAnydesk))
            {
                PathToAnydesk = "C:\\Program Files (x86)\\AnyDesk\\Anydesk.exe";
                Config.Settings.PathToAnydesk = PathToAnydesk;
            }

            if (string.IsNullOrEmpty(PathToAmmyAdmin))
            {
                PathToAmmyAdmin = "C:\\Program Files (x86)\\Ammy LLC\\Ammy Admin\\AA_v3.exe";
                Config.Settings.PathToAmmyAdmin = PathToAmmyAdmin;
            }

            if (string.IsNullOrEmpty(PathToAssistant))
            {
                PathToAssistant = "C:\\Program Files (x86)\\Ассистент\\assistant.exe";
                Config.Settings.PathToAssistant = PathToAssistant;
            }

            if (string.IsNullOrEmpty(PathToRustDesk))
            {
                PathToRustDesk = "C:\\Program Files\\RustDesk\\rustdesk.exe";
                Config.Settings.PathToRustDesk = PathToRustDesk;
            }
        }

        #region [Variables]

        string pathToCLEARbat;
        string pathToAnydesk;
        string pathToAmmyAdmin;
        string pathToAssistant;
        string pathToRustDesk;

        #endregion

        #region [Collections]

        public string PathToCLEARbat
        {
            get { return pathToCLEARbat; }
            set { pathToCLEARbat = value; OnPropertyChanged(nameof(PathToCLEARbat)); }
        }

        public string PathToAnydesk
        {
            get { return pathToAnydesk; }
            set { pathToAnydesk = value; OnPropertyChanged(nameof(PathToAnydesk)); }
        }

        public string PathToAmmyAdmin
        {
            get { return pathToAmmyAdmin; }
            set { pathToAmmyAdmin = value; OnPropertyChanged(nameof(PathToAmmyAdmin)); }
        }

        public string PathToAssistant
        {
            get { return pathToAssistant; }
            set { pathToAssistant = value; OnPropertyChanged(nameof(PathToAssistant)); }
        }

        public string PathToRustDesk
        {
            get { return pathToRustDesk; }
            set { pathToRustDesk = value; OnPropertyChanged(nameof(PathToRustDesk)); }
        }

        #endregion

        #region [Methods]

        public void SaveCleatbatPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToCLEARbat = path;
            Config.Settings.PathToCLEARbat = path;
        }

        public void SaveAnydeskPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToAnydesk =  path;
            Config.Settings.PathToAnydesk = path;
        }

        public void SaveAmmyAdminPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToAmmyAdmin = path;
            Config.Settings.PathToAmmyAdmin = path;
        }

        public void SaveAssistantPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToAssistant = path;
            Config.Settings.PathToAssistant = path;
        }

        public void SaveRustDeskPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToRustDesk = path;
            Config.Settings.PathToRustDesk = path;
        }

        #endregion
    }
}
