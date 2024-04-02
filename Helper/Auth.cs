using AqbaApp.API;
using AqbaApp.Model.Authorization;
using AqbaApp.View;
using System.Threading.Tasks;
using System;
using System.Windows;
using System.IdentityModel.Tokens.Jwt;

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
                    if (OpenAuthWindow())
                        return true;
                    else return false;
                }
                else return true;
            }
            else return true;
            

        }

        static async Task<bool> RefreshRefreshToken()
        {
            if (string.IsNullOrEmpty(Config.Settings.Token.RefreshToken))
                return false;

            AuthenticateResponse response = await Request.RefreshRefreshToken(Config.Settings.Token.RefreshToken);

            // Если обновление ключа произошло с ошибкой
            if (!TokenIsValid(response))
                return false;

            // Иначе сохранить данные
            Config.SaveToken(response);
            return true;
        }

        static bool OpenAuthWindow()
        {
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

        public static async Task<bool> LoginInService(string email, string password)
        {
            var token = await Request.Login(email, password);

            if (!TokenIsValid(token))
            {
                View.MessageBox.Show("Ошибка", "Не удалось войти", MessageBoxButton.OK);
                return false;
            }
            else
            {
                Config.SaveToken(token);
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
