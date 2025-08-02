using AqbaApp.Core;
using AqbaApp.Model.Client;
using AqbaApp.Service.Client;

namespace AqbaApp.ViewModel
{
    public class SettingsViewModel(SettingService<MainSettings> mainSettings) : NotifyProperty
    {
        #region [Collections]

        public string PathToCLEARbat
        {
            get => mainSettings.Settings.PathToCLEARbat;
            set 
            {
                mainSettings.Settings.PathToCLEARbat = value;
                OnPropertyChanged(nameof(PathToCLEARbat)); 
            }
        }

        public string PathToAnydesk
        {
            get => mainSettings.Settings.PathToAnydesk;
            set 
            {
                mainSettings.Settings.PathToAnydesk = value; 
                OnPropertyChanged(nameof(PathToAnydesk)); 
            }
        }

        public string PathToAmmyAdmin
        {
            get => mainSettings.Settings.PathToAmmyAdmin;
            set 
            {
                mainSettings.Settings.PathToAmmyAdmin = value; 
                OnPropertyChanged(nameof(PathToAmmyAdmin)); 
            }
        }

        public string PathToAssistant
        {
            get => mainSettings.Settings.PathToAssistant;
            set 
            {
                mainSettings.Settings.PathToAssistant = value; 
                OnPropertyChanged(nameof(PathToAssistant)); 
            }
        }

        public string PathToRustDesk
        {
            get => mainSettings.Settings.PathToRustDesk;
            set 
            {
                mainSettings.Settings.PathToRustDesk = value; 
                OnPropertyChanged(nameof(PathToRustDesk)); 
            }
        }

        #endregion

        #region [Methods]

        public void SaveCleatbatPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToCLEARbat = path;
            mainSettings.Settings.PathToCLEARbat = path;
        }

        public void SaveAnydeskPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToAnydesk =  path;
            mainSettings.Settings.PathToAnydesk = path;
        }

        public void SaveAmmyAdminPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToAmmyAdmin = path;
            mainSettings.Settings.PathToAmmyAdmin = path;
        }

        public void SaveAssistantPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToAssistant = path;
            mainSettings.Settings.PathToAssistant = path;
        }

        public void SaveRustDeskPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return;
            PathToRustDesk = path;
            mainSettings.Settings.PathToRustDesk = path;
        }

        #endregion
    }
}
