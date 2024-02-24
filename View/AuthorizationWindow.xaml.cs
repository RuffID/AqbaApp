using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AqbaApp.View
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
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
