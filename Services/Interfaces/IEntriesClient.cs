using System.Net;
using System.Threading.Tasks;
using XePlatformAuthentication.Models;

namespace XePlatformAuthentication.Services.Interfaces
{
    public interface IEntriesClient
    {
        Task<(HttpStatusCode statusCode, string result)> GetEntryAsync(Token token, string accountId, string entryId);
    }
}