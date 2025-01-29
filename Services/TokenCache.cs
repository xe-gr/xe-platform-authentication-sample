using System.Collections.Concurrent;
using System.Collections.Generic;
using XePlatformAuthentication.Models;
using XePlatformAuthentication.Services.Implementation;

namespace XePlatformAuthentication.Services
{
    public class TokenCache : ITokenCache
    {
        private static readonly ConcurrentDictionary<string, Token> Tokens = new();

        public void Save(Token token, string key)
        {
            Remove(key);
            Tokens.TryAdd(key, token);
        }

        public Token Get(string key)
        {
            return Tokens.GetValueOrDefault(key);
        }

        public void Remove(string key)
        {
            Tokens.TryRemove(key, out _);
        }
    }
}
