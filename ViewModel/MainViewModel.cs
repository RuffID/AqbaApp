using System.Windows;
using System.Windows.Controls;
using AqbaApp.Helper;
using AqbaApp.View;

namespace AqbaApp.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            //Config.Start();
            ButtonAccessibility = false;
            Access = new AccessPage();
            Report = new ReportPage();
            Settings = new SettingsPage();
            CurrentPage = Report;
            CurWindowState = WindowState.Normal;
            CurWindowStyle = WindowStyle.SingleBorderWindow;
            CheckElectronicQueueMode();
        }

        #region [Variables]

        private bool buttonAccessibility;
        private readonly Page Access;
        private readonly Page Report;
        private readonly Page Settings;
        private Page _currentPage;
        private WindowState _curWindowState;
        private WindowStyle _curWindowStyle;
        private RelayCommand _changeWindow;
        RelayCommand openLoginPage;
        RelayCommand openReportPage;
        RelayCommand openSettingsPage;
        RelayCommand closingWindow;
        RelayCommand mainWindowLoaded;

        #endregion

        #region [Properties]        

        public bool ButtonAccessibility { 
            get { return buttonAccessibility; } 
            set { buttonAccessibility = value; OnPropertyChanged(nameof(ButtonAccessibility)); } 
        }

        public Page CurrentPage
        {
            get => _currentPage;
            set => Set(ref _currentPage, value);
        }

        public WindowState CurWindowState
        {
            get => _curWindowState;
            set
            {
                _curWindowState = value;
                OnPropertyChanged();
            }
        }

        public WindowStyle CurWindowStyle
        {
            get => _curWindowStyle;
            set
            {
                _curWindowStyle = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region [Commands]
                
        public RelayCommand OpenLoginPage
        {
            get
            {
                return openLoginPage ??= new RelayCommand((o) => CurrentPage = Access);
            }
        }        

        public RelayCommand OpenReportPage
        {
            get
            {
                return openReportPage ??= new RelayCommand((o) => CurrentPage = Report);
            }
        }

        public RelayCommand OpenSettingsPage
        {
            get
            {
                return openSettingsPage ??= new RelayCommand((o) => CurrentPage = Settings);
            }
        }

        public RelayCommand ChangeWindowState
        {
            get
            {
                return _changeWindow ??= new RelayCommand((o) =>
                {
                    if (CurWindowState == WindowState.Normal)
                    {
                        CurWindowStyle = WindowStyle.None;
                        CurWindowState = WindowState.Maximized;
                    }
                    else
                    {
                        CurWindowStyle = WindowStyle.SingleBorderWindow;
                        CurWindowState = WindowState.Normal;
                    }
                });
            }
        }
        
        public RelayCommand ClosingWindow
        {
            get
            {
                return closingWindow ??= new RelayCommand((o) =>
                {
                    Config.SaveOrCreateConfig(ref Config.Settings);
                    Config.SaveOrCreateConfig(ref Config.Cache);
                });
            }
        }

        public RelayCommand MainWindowLoaded
        {
            get
            {
                return mainWindowLoaded ??= new RelayCommand(async (o) =>
                {
                    ButtonAccessibility = false;

                    await Update.CheckUpdate();
                    
                    ButtonAccessibility = true;
                });
            }
        }

        #endregion

        #region [Methods]

        private void CheckElectronicQueueMode()
        {
            if (Config.Settings.ElectronicQueueMode)
            {
                CurWindowStyle = WindowStyle.None;
                CurWindowState = WindowState.Maximized;
            }
        }

        #endregion
    }
}