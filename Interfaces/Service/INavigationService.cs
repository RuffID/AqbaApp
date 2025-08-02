using System.Windows.Controls;
using System.Windows;

namespace AqbaApp.Interfaces.Service
{
    public interface INavigationService
    {
        void NavigateToPage<TPage>() where TPage : Page;
        void OpenWindow<TWindow>() where TWindow : Window;
        void SetMainFrame(Frame frame);
        bool? OpenDialog<TWindow>() where TWindow : Window;
    }
}
