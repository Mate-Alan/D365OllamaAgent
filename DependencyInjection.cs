using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

public static class DependencyInjection
{
    public static IServiceCollection AddFunctionDependencies(this IServiceCollection services)
    {
        // Register DefaultAzureCredential as a singleton.
        // This provides credentials for authenticating with Azure services using Managed Identity or other mechanisms.
        services.AddSingleton(new DefaultAzureCredential());
        
        // Register IOrganizationService with an implementation of ServiceClient.
        // This will handle connections to Dataverse using tokens provided by the DefaultAzureCredential.
        services.AddSingleton<IOrganizationService, ServiceClient>(provider =>
        {
            // Retrieve the DefaultAzureCredential instance from the service provider.
            var managedIdentity = provider.GetRequiredService<DefaultAzureCredential>();

            // Read the Dataverse environment URL from environment variables.
            var environment = Environment.GetEnvironmentVariable("CrmConnection:Url");

            // Retrieve the IMemoryCache instance (optional, may be null if not registered).
            var cache = provider.GetService<IMemoryCache>();

            return new ServiceClient(
                tokenProviderFunction: f => GetToken(environment, managedIdentity, cache), // Custom function to retrieve an access token.
                instanceUrl: new Uri(environment),
                useUniqueInstance: true 
            );
        });

        services.AddSingleton<ICrmDataAccess, CrmDataAccess>();
        
        return services;
    }

    private static async Task<string> GetToken(string environment, DefaultAzureCredential credential, IMemoryCache memoryCache)
    {
        // Attempt to retrieve the token from the cache or create a new entry if it doesn't exist.
        var accessToken = await memoryCache.GetOrCreateAsync(environment, async (cacheEntry) =>
        {
            // Set the cache expiration to 50 minutes
            cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(50);

            // Request a new access token from Azure using the DefaultAzureCredential.
            var token = await credential.GetTokenAsync(
                new TokenRequestContext(new[] { $"{environment}/.default" }) // Define the scope for the token request.
            );

            return token; 
        });

        return accessToken.Token;
    }
}
