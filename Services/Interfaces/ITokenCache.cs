using XePlatformAuthentication.Models;

namespace XePlatformAuthentication.Services.Interfaces
{
    public interface ITokenCache
    {
        void Save(Token token, string key);
        Token Get(string key);
        void Remove(string key);
    }
}