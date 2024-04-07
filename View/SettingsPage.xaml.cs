using AqbaApp.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AqbaApp.View
{
    /// <summary>
    /// Логика взаимодействия для SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

  
        private void SaveClearBatPath_OnClick(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            var dialog = new OpenFileDialog
            {
                Title = "Путь до CLEAR.bat.exe",
                Filter = "CLEAR.bat|*.exe|Любой файл (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                (DataContext as SettingsViewModel)?.SaveCleatbatPath(dialog.FileName);
        }

        private void SaveAnyDeskPath_OnClick(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            var dialog = new OpenFileDialog
            {
                Title = "Путь до Anydesk.exe",
                Filter = "Anydesk|*.exe|Любой файл (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                (DataContext as SettingsViewModel)?.SaveAnydeskPath(dialog.FileName);
        }

        private void SaveAmmyAdminPath_OnClick(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            var dialog = new OpenFileDialog
            {
                Title = "Путь до AmmyAdmin.exe",
                Filter = "Ammy admin|*.exe|Любой файл (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                (DataContext as SettingsViewModel)?.SaveAmmyAdminPath(dialog.FileName);
        }

        private void SaveAssistant_OnClick(object sender, RoutedEventArgs e)
        {
            ArgumentNullException.ThrowIfNull(e);

            var dialog = new OpenFileDialog
            {
                Title = "Путь до assistant.exe",
                Filter = "Assistant|*.exe|Любой файл (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                (DataContext as SettingsViewModel)?.SaveAssistantPath(dialog.FileName);
        }
    }
}
