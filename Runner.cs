using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using XePlatformAuthentication.Services.Implementation;

namespace XePlatformAuthentication
{
    public class Runner(IJwtClient jwtClient, IEntriesClient entriesClient, ITokenCache tokenCache)
    {
        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private string _userName = string.Empty;
        private string _password = string.Empty;

        public async Task Run()
        {
            var choice = Choice.None;

            while (choice != Choice.Exit)
            {
                choice = await ChooseActionAsync();

                if (choice == Choice.GetEntry)
                {
                    await GetEntryAsync();
                }
            }
        }

        private async Task<Choice> ChooseActionAsync()
        {
            if (string.IsNullOrEmpty(_clientId))
            {
                await ReadParametersAsync();
            }

            Console.WriteLine("1. Retrieve ad/entry of customer");
            Console.WriteLine("2. Exit");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1": return Choice.GetEntry;
                case "2": return Choice.Exit;
                default: return Choice.None;
            }
        }

        private async Task ReadParametersAsync()
        {
            while (string.IsNullOrEmpty(_clientId))
            {
                Console.Write("Enter client_id: ");
                _clientId = Console.ReadLine();
                Console.Write("Enter client_secret: ");
                _clientSecret = Console.ReadLine();
                Console.Write("Enter username: ");
                _userName = Console.ReadLine();
                Console.Write("Enter password: ");
                _password = Console.ReadLine();

                await GetToken();

                Console.WriteLine();
            }
        }

        private async Task<bool> GetToken()
        {
            var response = await jwtClient.GetTokenAsync(_clientId, _clientSecret, _userName, _password);

            if (response.statusCode == HttpStatusCode.OK)
            {
                tokenCache.Save(response.token, Globals.Constants.TokenCacheKey);

                return true;
            }

            Console.WriteLine($"Failed to get token, status code {response.statusCode}");
            Console.WriteLine(response.body);
            _clientId = string.Empty;

            return false;
        }

        private async Task GetEntryAsync()
        {
            if (tokenCache.Get(Globals.Constants.TokenCacheKey) is null)
            {
                Console.WriteLine("Retrieving token");

                if (!await GetToken())
                {
                    Console.WriteLine("Failed to retrieve a token");
                    return;
                }
            }

            Console.Write("Please provide the id of the entry to retrieve: ");

            var entryId = Console.ReadLine();

            Console.Write("Please provide the account id of your customer who owns the entry to retrieve: ");

            var accountId = Console.ReadLine();

            var response = await entriesClient.GetEntryAsync(tokenCache.Get(Globals.Constants.TokenCacheKey), accountId, entryId);

            if (response.statusCode == HttpStatusCode.Unauthorized)
            {
                tokenCache.Remove(Globals.Constants.TokenCacheKey);
            }

            string formattedJson;

            try
            {
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(response.result);
                formattedJson = JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }
            catch (Exception)
            {
                formattedJson = response.result;
            }

            if (response.statusCode != HttpStatusCode.OK)
            {
                Console.WriteLine($"Status code: {response.statusCode}");
            }

            Console.WriteLine(formattedJson);
        }

        internal enum Choice
        {
            GetEntry,
            Exit,
            None
        }
    }
}