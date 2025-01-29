using System.Net;
using System.Threading.Tasks;
using RestSharp;
using XePlatformAuthentication.Models;
using XePlatformAuthentication.Services.Implementation;

namespace XePlatformAuthentication.Services
{
    public class JwtClient(AppSettings settings) : IJwtClient
    {
        public async Task<(HttpStatusCode statusCode, Token token, string body)> GetTokenAsync(string clientId, string clientSecret, string userName, string password)
        {
            using (var client = new RestClient(settings.XeAuthUrl))
            {
                var request = new RestRequest("/auth/realms/xe/protocol/openid-connect/token", Method.Post);

                request.AddParameter("client_id", clientId);
                request.AddParameter("client_secret", clientSecret);
                request.AddParameter("username", userName);
                request.AddParameter("password", password);
                request.AddParameter("grant_type", "password");
                request.AddParameter("scope", "openid profile email");

                request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                var response = await client.ExecuteAsync<Token>(request);

                if (response.IsSuccessStatusCode)
                {
                    return (HttpStatusCode.OK, response.Data, string.Empty);
                }

                return (response.StatusCode, null, response.Content);
            }
        }
    }
}
