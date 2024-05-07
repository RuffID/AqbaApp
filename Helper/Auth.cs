using AqbaApp.API;
using AqbaApp.Model.Authorization;
using AqbaApp.View;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.IdentityModel.Tokens.Jwt;
using Notifications.Wpf.Core;

namespace AqbaApp.Helper
{
    public class Auth
    {
        public static async Task<bool> CheckAndRefreshToken()
        {
            // Если истёк срок годности ключа доступа либо он не сохранён в конфиге, то
            if (!TokenIsValid(Config.Settings.Token))
            {
                // Обновление ключа обновления и получение вместе с ним ключа доступа
                if (!await RefreshRefreshToken())
                {
                    // Если через refresh токен не удалось обновить, то повторная авторизация
                    AuthorizationWindow Login = new();

                    while (true)
                    {
                        bool? result = Login.ShowDialog();
                        if (result == null || result == false)
                            return false;
                        else if (result == true)
                            return true;
                    }
                }
                else return true;
            }
            else return true;
        }

        static async Task<bool> RefreshRefreshToken()
        {
            if (string.IsNullOrEmpty(Config.Settings.Token.RefreshToken))
                return false;

            var response = await Request.RefreshRefreshToken(Config.Settings.Token.RefreshToken);

            // Если во время отправки запроса на обновление токена возникла ошибка, то возвращает true, чтобы не выходило окно ввода логина и пароля
            if (!response.RequestSuccessful)
            {
                _ = Notice.Show(NotificationType.Warning, "Не удалось обновить refresh токен.\nПроверьте связь с сервером.");
                return true;
            }

            // Если обновление ключа произошло с ошибкой
            if (!TokenIsValid(response.Response))
                return false;

            // Иначе сохранить данные
            Config.SaveToken(response.Response);
            return true;
        }

        public static async Task<bool> LoginInService(string email, string password)
        {
            var token = await Request.Login(email, password);

            if (token.RequestSuccessful == false)
            {
                _ = Notice.Show(NotificationType.Warning, "Не удалось отправить запрос на вход.\nПроверьте связь с сервером.");
                return false;
            }

            if (!TokenIsValid(token.Response))
            {
                View.MessageBox.Show("", "Не удалось войти в систему.\nПроверьте логин или пароль.", MessageBoxButton.OK);
                return false;
            }
            else
            {
                _ = Notice.Show(NotificationType.Success, "Успешный вход!");
                Config.SaveToken(token.Response);
                return true;
            }
        }

        static bool TokenIsValid(AuthenticateResponse token)
        {
            if (token == null || string.IsNullOrEmpty(token.RefreshToken) || string.IsNullOrEmpty(token.AccessToken))
                return false;

            JwtSecurityToken decodeJWT;

            try
            {
                decodeJWT = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);
            }
            catch (Exception e)
            {
                WriteLog.Error(e.ToString());
                return false;
            }            

            if (decodeJWT.ValidTo < DateTime.UtcNow)
                return false;
            else return true;
        }
    }
}
