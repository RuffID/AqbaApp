using AqbaApp.Core;
using AqbaApp.Interfaces.Api;
using AqbaApp.Model.Authorization;
using AqbaApp.Model.Client;
using AqbaApp.Service.Client;
using AqbaApp.View;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace AqbaApp.ViewModel
{
    public class AuthorizationViewModel : NotifyProperty
    {
        #region [Variables]

        private readonly ILogger<AuthorizationViewModel> _logger;
        private readonly IRequestService _request;
        private readonly Lazy<RelayCommand> _loginCommand;
        private readonly SettingService<MainSettings> _settings;
        private readonly string loginLink;
        private string? login;
        private string? password;
        private bool btnLoginActive;

        #endregion

        public AuthorizationViewModel(SettingService<MainSettings> settings, Immutable immutable, IRequestService request, ILoggerFactory logger) 
        {
            _logger = logger.CreateLogger<AuthorizationViewModel>();
            _settings = settings;
            _request = request;
            _loginCommand = new Lazy<RelayCommand>(() => new RelayCommand(async o => await LoginMethod(o)));
            loginLink = $"{_settings.Settings.ServerAddress}/{immutable.ApiLoginEndpoint}/login";
            btnLoginActive = true;

        }

        #region [Collections]        

        public string? Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string? Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        public bool LoginActive
        {
            get => btnLoginActive;
            set 
            { 
                btnLoginActive = value; 
                OnPropertyChanged(nameof(LoginActive)); 
            }
        }

        #endregion

        #region [Commands]

        public RelayCommand? LoginCommand => _loginCommand.Value;

        #endregion

        #region [Methods]

        private async Task LoginMethod(object? o)
        {
            if (o == null || o is not AuthorizationWindow window)
                return;

            LoginActive = false;

            if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
            {
                View.MessageBox.Show("Предупреждение", "Поле логина или пароля не может быть пустым.", MessageBoxButton.OK);
                LoginActive = true;
                return;
            }

            string? response = await _request.SendPost(loginLink + $"?login={Login}&password={Password}", null);

            if (string.IsNullOrEmpty(response) || response.Contains("incorrect", StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogWarning("Error sending login request. Possibly incorrect login or password.");    
                View.MessageBox.Show("Предупреждение", "Ошибка при входе в систему. Возможно неправильный логин или пароль.", MessageBoxButton.OK);
                _settings.Settings.Token = null;
            }
            else
                _settings.Settings.Token = DeserializeToken(response);

            if (!string.IsNullOrEmpty(_settings.Settings.Token?.AccessToken))
            {
                window.DialogResult = true;
                window.Close();
            }

            LoginActive = true;
        }

        private AuthenticateResponse? DeserializeToken(string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<AuthenticateResponse>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deserializing bearer token.");
                return null;
            }
        }

        #endregion

    }
}
