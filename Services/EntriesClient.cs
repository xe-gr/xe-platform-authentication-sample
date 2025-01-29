using System.Net;
using System.Threading.Tasks;
using RestSharp;
using XePlatformAuthentication.Models;
using XePlatformAuthentication.Services.Implementation;

namespace XePlatformAuthentication.Services
{
    public class EntriesClient(AppSettings settings) : IEntriesClient
    {
        public async Task<(HttpStatusCode statusCode, string result)> GetEntryAsync(Token token, string accountId, string entryId)
        {
            using (var client = new RestClient(settings.XeApiUrl))
            {
                var request = new RestRequest($"/entries/v12/{accountId}/{entryId}");
                request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
                var response = await client.ExecuteAsync(request);
                return (response.StatusCode, response.Content);
            }
        }
    }
}