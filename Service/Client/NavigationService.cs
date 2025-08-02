using AqbaApp.Interfaces.Service;
using System.Windows.Controls;
using System.Windows;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace AqbaApp.Service.Client
{
    public class NavigationService(IServiceProvider serviceProvider) : INavigationService
    {
        private Frame? _mainFrame;

        public void SetMainFrame(Frame frame)
        {
            _mainFrame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void NavigateToPage<TPage>() where TPage : Page
        {
            if (_mainFrame == null)
                throw new InvalidOperationException("Main frame not selected.");

            // Проверяем, не активна ли уже эта страница
            if (_mainFrame.Content is TPage)
                return;

            var page = serviceProvider.GetService<TPage>();

            if (page == null)
                throw new InvalidOperationException($"Page {typeof(TPage).Name} not registered in DI.");

            _mainFrame.Navigate(page);
        }

        public void OpenWindow<TWindow>() where TWindow : Window
        {
            var window = serviceProvider.GetRequiredService<TWindow>();

            if (window == null)
                throw new InvalidOperationException($"Window {typeof(TWindow).Name} not registered in DI.");

            window.Show();
        }

        public bool? OpenDialog<TWindow>() where TWindow : Window
        {
            var window = serviceProvider.GetRequiredService<TWindow>();

            if (window == null)
                throw new InvalidOperationException($"Window {typeof(TWindow).Name} not registered in DI.");

            return window.ShowDialog();
        }
    }
}
