/*namespace AqbaClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public LoginViewModel() { }

        RelayCommand login;
        public RelayCommand Login
        {
            get
            {
                return login ??= new RelayCommand((o) =>
                {
                    Config.Settings.OkdeskLogin = LoginText;
                    Config.Settings.OkdeskPassword = Password;
                });
            }
        }

        string passwordText;
        public string Password
        {
            private get { return passwordText; }
            set
            {
                passwordText = value;
                OnPropertyChanged();
            }
        }

        string loginText;
        public string LoginText 
        {
            get { return loginText; }
            set
            {
                loginText = value;
                OnPropertyChanged();
            }
        }
    }
}*/