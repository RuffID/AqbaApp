using AqbaApp.Interfaces.Api;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AqbaApp.Core.Api
{
    public class RequestClient : IRequestService
    {
        private readonly ILogger<RequestClient> _logger;
        private readonly HttpClient _httpClient;

        public RequestClient(HttpClient httpClient, ILoggerFactory logger)
        {
            _httpClient = httpClient;
            // Бесконечный таймаут для httpclient на debug режим
#if DEBUG
            _httpClient.Timeout = Timeout.InfiniteTimeSpan;
#else
            _httpClient.Timeout = TimeSpan.FromSeconds(100);
#endif
            _logger = logger.CreateLogger<RequestClient>();
        }

        public async Task<string?> SendPost(string link, StringContent? content, [CallerMemberName] string caller = "")
        {
            try
            {
                using var response = await _httpClient.PostAsync(link, content);
                
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Method:{MethodName}] Error while send post async request.. Caller: {CallerMethod}", nameof(SendGet), caller);
                return null;
            }
        }

        public async Task<string?> SendPut(string link, StringContent? content, string? bearerToken = null, [CallerMemberName] string caller = "")
        {
            if (!string.IsNullOrEmpty(bearerToken))
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", bearerToken);

            try
            {
                using var response = await _httpClient.PutAsync(link, content);

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Method:{MethodName}] Error while send put async request. Caller: {CallerMethod}", nameof(SendGet), caller);
                return null;
            }
        }

        public async Task<string?> SendGet(string link, string? bearerToken = null, [CallerMemberName] string caller = "")
        {
            if (!string.IsNullOrEmpty(bearerToken))
                _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", bearerToken);

            try
            {
                using HttpResponseMessage response = await _httpClient.GetAsync(link);
                string responseString = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _logger.LogWarning("[Method:{MethodName}] Authorize error. Error: {Error}. Link: {Link}. Caller: {CallerMethod}", nameof(SendGet), responseString, link, caller);
                    return null;
                }
                if (response.IsSuccessStatusCode == false)
                {
                    _logger.LogWarning("[Method:{MethodName}] Not success response code. Link: {Link}. Caller: {CallerMethod}", nameof(SendGet), link, caller);
                    return null;
                }

                return responseString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Method:{MethodName}] Error while send get async request. Caller: {CallerMethod}", nameof(SendGet), caller);
                return null;
            }
        }

        public async Task<Stream?> SendGetStream(string fileUrl, [CallerMemberName] string caller = "")
        {
            try
            {
                return await _httpClient.GetStreamAsync(fileUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Method:{MethodName}] Error while send get filestream async request. Caller: {CallerMethod}", nameof(SendGetStream), caller);
                return Stream.Null;
            }
        }
    }
}
