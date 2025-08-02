using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AqbaApp.Interfaces.Api
{
    public interface IRequestService
    {
        Task<string?> SendPost(string link, StringContent? content, [CallerMemberName] string caller = "");
        Task<string?> SendPut(string link, StringContent? content, string? bearerToken = null, [CallerMemberName] string caller = "");
        Task<string?> SendGet(string link, string? bearerToken = null, [CallerMemberName] string caller = "");
        Task<Stream?> SendGetStream(string fileUrl, [CallerMemberName] string caller = "");
    }
}
