using AqbaApp.Helper;
using AqbaApp.View;
using System.Windows;

namespace AqbaApp.ViewModel
{
    public class AuthorizationViewModel : ViewModelBase
    {
        public AuthorizationViewModel() { }

        #region [Variables]

        private string login;
        private string password;
        private bool btnLoginActive = true;
        private RelayCommand loginCommand;

        #endregion

        #region [Collections]        

        public string Login
        {
            get => login;
            set
            {
                login = value;
                OnPropertyChanged();
            }
        }

        public string Password
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

        public RelayCommand LoginCommand
        {
            get
            {
                return loginCommand ??= new RelayCommand(async (o) =>
                {
                    LoginActive = false;
                    if (string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(Password))
                    {
                        View.MessageBox.Show("", "Поля логина и пароля не могут быть пустыми", MessageBoxButton.OK);
                        LoginActive = true;
                        return;
                    }

                    if (o != null && o is AuthorizationWindow window)
                    {
                        if (await Auth.LoginInService(Login, Password))
                        {
                            window.DialogResult = true;
                            window.Close();
                        }
                    }
                    LoginActive = true;
                });
            }
        }

        #endregion

    }
}
