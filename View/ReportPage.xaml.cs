﻿using AqbaApp.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace AqbaApp.View
{
    /// <summary>
    /// Логика взаимодействия для ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        public ReportPage()
        {
            InitializeComponent();
        }

        private void SaveBackgroundPath_OnClick(object sender, RoutedEventArgs e)
        {
            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            var dialog = new OpenFileDialog
            {
                Title = "Выберите картинку для фона",
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.gif|Любое расширение (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
                (DataContext as ReportViewModel)?.SaveBackgroundPath(dialog.FileName);
        }

        private void OpenTaskList_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth; // take into account vertical scrollbar
            var col1 = 0.45;
            var col2 = 0.25;
            var col3 = 0.30;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
        }
    }
}