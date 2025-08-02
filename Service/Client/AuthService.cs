using System.Threading.Tasks;
using System;
using System.IdentityModel.Tokens.Jwt;
using AqbaApp.Model.Client;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;
using AqbaApp.Interfaces.Api;
using AqbaApp.Model.Authorization;
using Newtonsoft.Json;
using AqbaApp.Interfaces.Service;
using AqbaApp.View;
using System.Net.Http;
using System.Text;

namespace AqbaApp.Service.Client
{
    public class AuthService(INavigationService navigate, SettingService<MainSettings> settings, IRequestService request, ILoggerFactory logger)
    {
        private readonly ILogger<AuthService> _logger = logger.CreateLogger<AuthService>();
        private readonly string link = $"{settings.Settings.ServerAddress}/api/crm_login/login/update_tokens?refresh_token=";

        public async Task<bool> CheckAndUpdateTokens()
        {
            // Если токены невалидные, то обновляет и возвращает результат обновления, true - получилось, false - ошибка
            if (!IsTokenValid())
            {
                if (!await UpdateRefreshToken())
                {
                    bool? response = navigate.OpenDialog<AuthorizationWindow>();
                    if (response == null || response == false)
                        return false;
                }
            }

            // Если токены валидные, то возвращает true - всё хорошо
            return true;
        }

        private async Task<bool> UpdateRefreshToken()
        {
            if (string.IsNullOrEmpty(settings.Settings.Token?.RefreshToken))
                return false;

            AuthenticateResponse? token = null;
            RefreshModel refreshModel = new(settings.Settings.Token.RefreshToken);
            var json = JsonConvert.SerializeObject(refreshModel);
            StringContent content = new(json, Encoding.UTF8, "application/json");

            string? response = await request.SendPut(link, content: content);

            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    token = JsonConvert.DeserializeObject<AuthenticateResponse>(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing bearer token.");
                    return false;
                }
            }

            // Если обновление ключа произошло с ошибкой
            if (token == null)
            {
                _logger.LogWarning("Failed to refresh tokens.");
                return false;
            }

            // Иначе сохранить данные по токену
            settings.Settings.Token = token;
            return true;
        }

        private bool IsTokenValid(string? accessToken = null)
        {
            string tempToken = string.Empty;

            if (!string.IsNullOrEmpty(accessToken))
                tempToken = accessToken;
            else if (!string.IsNullOrEmpty(settings.Settings.Token?.AccessToken))
                tempToken = settings.Settings.Token.AccessToken;
            else return false;


            JwtSecurityTokenHandler handler = new();
            JwtSecurityToken token;

            try
            {
                token = new JwtSecurityTokenHandler().ReadJwtToken(tempToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Method:{MethodName}] Error checking bearer token.", nameof(IsTokenValid));
                return false;
            }

            Claim? expClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim == null || !long.TryParse(expClaim.Value, out var exp))
                return false;

            DateTimeOffset expiryDate = DateTimeOffset.FromUnixTimeSeconds(exp);

            return expiryDate > DateTimeOffset.UtcNow;
        }
    }
}
