using System;
using System.Windows;
using System.Windows.Controls;

namespace AqbaApp.View
{
    /// <summary>
    /// Логика взаимодействия для MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        MessageBoxResult Result = MessageBoxResult.None;
        public MessageBox()
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
        }

        void AddButtons(MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    AddButton("ОК", MessageBoxResult.OK);
                    break;
                case MessageBoxButton.OKCancel:
                    AddButton("ОК", MessageBoxResult.OK);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                case MessageBoxButton.YesNo:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    break;
                case MessageBoxButton.YesNoCancel:
                    AddButton("Да", MessageBoxResult.Yes);
                    AddButton("Нет", MessageBoxResult.No);
                    AddButton("Отмена", MessageBoxResult.Cancel, isCancel: true);
                    break;
                default:
                    throw new ArgumentException("Unknown button value", nameof(buttons));
            }
        }

        void AddButton(string text, MessageBoxResult result, bool isCancel = false)
        {
            var button = new Button() { Content = text, IsCancel = isCancel };
            button.Click += (o, args) => { Result = result; DialogResult = true; };
            ButtonContainer.Children.Add(button);
        }

        public static MessageBoxResult Show(string caption, string message, MessageBoxButton buttons)
        {
            var dialog = new MessageBox() { Title = caption };
            dialog.MessageContainer.Text = message;
            dialog.AddButtons(buttons);
            dialog.ShowDialog();
            return dialog.Result;
        }
    }
}
