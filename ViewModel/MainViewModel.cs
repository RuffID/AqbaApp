using System;
using System.Windows;
using AqbaApp.Core;
using AqbaApp.Interfaces;
using AqbaApp.Interfaces.Service;
using AqbaApp.Model.Client;
using AqbaApp.Service.Client;
using AqbaApp.View;

namespace AqbaApp.ViewModel
{
    public class MainViewModel : NotifyProperty
    {
        #region [Variables]

        private readonly SettingService<MainSettings> _settings;
        private readonly INavigationService _navigationService;
        private readonly Lazy<RelayCommand> _openAccessPage;
        private readonly Lazy<RelayCommand> _openReportPage;
        private readonly Lazy<RelayCommand> _openSettingPage;
        private bool buttonAccessibility;        
        private WindowState _curWindowState;
        private WindowStyle _curWindowStyle;

        #endregion

        public MainViewModel(SettingService<MainSettings> mainSettings, INavigationService navigationService)
        {
            _settings = mainSettings;
            _navigationService = navigationService;
            ButtonAccessibility = false;
            CurWindowState = WindowState.Normal;
            CurWindowStyle = WindowStyle.SingleBorderWindow;

            _openAccessPage = new Lazy<RelayCommand>(() => new RelayCommand(o => _navigationService.NavigateToPage<AccessPage>()));
            _openReportPage = new Lazy<RelayCommand>(() => new RelayCommand(o => _navigationService.NavigateToPage<ReportPage>()));
            _openSettingPage = new Lazy<RelayCommand>(() => new RelayCommand(o => _navigationService.NavigateToPage<SettingsPage>()));

            CheckElectronicQueueMode();
        }

        #region [Properties]

        public bool IsAuthenticated => !string.IsNullOrEmpty(_settings.Settings.Token?.AccessToken);

        public string LoginIcon => IsAuthenticated ? @"Resources/Icons/logout.png" : @"Resources/Icons/login.png";

        public bool ButtonAccessibility 
        { 
            get => buttonAccessibility;
            set 
            { 
                buttonAccessibility = value; 
                OnPropertyChanged(nameof(ButtonAccessibility)); 
            } 
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

        public RelayCommand OpenAccessPage => _openAccessPage.Value;

        public RelayCommand OpenReportPage => _openReportPage.Value;

        public RelayCommand OpenSettingsPage => _openSettingPage.Value;

        public RelayCommand ChangeWindowState
        {
            get
            {
                return new RelayCommand((o) =>
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

        public RelayCommand ExitOrLoginInService
        {
            get
            {
                return new RelayCommand((o) =>
                {
                    // Если пользователь залогинен в систему (токен сохранён/он не равен null)
                    if (IsAuthenticated)
                    {
                        // То необходимо выйти из системы
                        if (_settings.Settings.Token != null)
                        {
                            _settings.Settings.Token.AccessToken = string.Empty;
                            OnPropertyChanged(nameof(IsAuthenticated));
                            OnPropertyChanged(nameof(LoginIcon));
                        }
                    }

                    // И в любом случае (при входе или выходе из системы) необходимо открыть окно для входа
                    if (_navigationService.OpenDialog<AuthorizationWindow>() == true && !string.IsNullOrEmpty(_settings.Settings.Token?.AccessToken))
                    {
                        OnPropertyChanged(nameof(IsAuthenticated));
                        OnPropertyChanged(nameof(LoginIcon));
                    }
                });
            }
        }

        #endregion

        private void CheckElectronicQueueMode()
        {
            if (_settings.Settings.ElectronicQueueMode)
            {
                CurWindowStyle = WindowStyle.None;
                CurWindowState = WindowState.Maximized;
            }
        }
    }
}