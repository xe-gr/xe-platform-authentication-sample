using System;

namespace XePlatformAuthentication.Helpers
{
    public static class EnvironmentExtensions
    {
        private const string DevelopmentEnvironment = "Development";

        public static string GetEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (string.IsNullOrEmpty(environment))
            {
                environment = DevelopmentEnvironment;
            }

            return environment;
        }
    }
}
