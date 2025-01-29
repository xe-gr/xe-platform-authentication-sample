using System.Net;
using System.Threading.Tasks;
using XePlatformAuthentication.Models;

namespace XePlatformAuthentication.Services.Interfaces
{
    public interface IJwtClient
    {
        Task<(HttpStatusCode statusCode, Token token, string body)> GetTokenAsync(string clientId, string clientSecret, string userName, string password);
    }
}
