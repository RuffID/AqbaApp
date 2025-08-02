using AqbaApp.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace AqbaApp.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow(AuthorizationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            Owner = Application.Current.MainWindow;
        }

        private void Pass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Pass.Password.Length > 0)
                tbPass.Visibility = Visibility.Collapsed;
            else tbPass.Visibility = Visibility.Visible;

            if (DataContext != null)
            { ((dynamic)DataContext).Password = ((PasswordBox)sender).Password; }
        }
    }
}
