using AqbaApp.Interfaces;
using AqbaApp.Interfaces.Api;
using AqbaApp.Interfaces.Service;
using AqbaApp.Model.Client;
using AqbaApp.Service.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AqbaApp.Core.Api
{
    public class GetItemService
    {
        private readonly ILogger<GetItemService> _logger;
        private readonly IRequestService _requestService;
        private readonly SettingService<MainSettings> _mainSettings;
        private readonly AuthService authService;

        public GetItemService(ILoggerFactory logger, IHttpClientFactory client, INavigationService navigate, SettingService<MainSettings> mainSettings)
        {
            _logger = logger.CreateLogger<GetItemService>();
            _requestService = new RequestClient(client.CreateClient(), logger);
            _mainSettings = mainSettings;
            authService = new(navigate, mainSettings, _requestService, logger);
        }

        public async IAsyncEnumerable<List<T>> GetAllItems<T>(string link, long startIndex, long limit, [CallerMemberName] string caller = "")
        {
            while (true)
            {
                List<T>? collection = await GetRangeOfItems<T>(link, startIndex, caller);

                if (collection == null || collection.Count == 0)
                    yield break;

                if (collection.Last() is IEntity entity)
                    startIndex = entity.Id + 1;

                yield return collection;

                if (collection.Count < limit)
                    yield break;
            }
        }

        public async Task<List<T>?> GetRangeOfItems<T>(string link, long startIndex = 0, [CallerMemberName] string caller = "")
        {
            if (!await authService.CheckAndUpdateTokens())
                return null;

            string tempLink = link;

            if (startIndex > 0)
                tempLink = link + $"?startIndex={startIndex}";

            string? response = await _requestService.SendGet(tempLink, _mainSettings.Settings.Token?.AccessToken);

            if (string.IsNullOrEmpty(response) || response == "[]")
                return null;            

            try
            {
                return JsonConvert.DeserializeObject<List<T>>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Method: {MethodName}, caller: {CallerMethod}", nameof(GetRangeOfItems), caller);
                return null;
            }
        }

        public async Task<T?> GetItem<T>(string link, [CallerMemberName] string caller = "")
        {
            if (!await authService.CheckAndUpdateTokens())
                return default;

            string? response = await _requestService.SendGet(link, _mainSettings.Settings.Token?.AccessToken);

            if (string.IsNullOrEmpty(response) || response == "[]")
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Method: {MethodName}, caller: {CallerMethod}", nameof(GetItem), caller);
                return default;
            }
        }   
    }
}