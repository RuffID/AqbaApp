using Notifications.Wpf.Core;
using System;
using System.Threading.Tasks;

namespace AqbaApp.Helper
{
    public static class Notice
    {
        public static async Task Show(NotificationType notificationType, string message, int expirationTimeInSeconds = 5)
        {
            var notificationContent = new NotificationContent();

            switch (notificationType)
            {
                case NotificationType.Information:
                    notificationContent.Type = NotificationType.Information;
                    notificationContent.Title = "Информация";
                    break;
                case NotificationType.Warning:
                    notificationContent.Type = NotificationType.Warning;
                    notificationContent.Title = "Предупреждение";
                    break;
                case NotificationType.Error:
                    notificationContent.Type = NotificationType.Error;
                    notificationContent.Title = "Ошибка";
                    break;
                case NotificationType.Success:
                    notificationContent.Type = NotificationType.Success;
                    notificationContent.Title = "Успех!";
                    break;
                default:
                    break;
            }
            notificationContent.Message = message;            

            var notificationManager = new NotificationManager( );            
            await notificationManager.ShowAsync(notificationContent, areaName: "NotificationWindowArea", expirationTime: TimeSpan.FromSeconds(expirationTimeInSeconds));
        }
    }
}
