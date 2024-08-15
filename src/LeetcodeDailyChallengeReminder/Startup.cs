using System.Text.Json;
using Amazon.SecretsManager.Extensions.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace LeetcodeDailyChallenge;

[Amazon.Lambda.Annotations.LambdaStartup]
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<ISecretsManagerCache>(new SecretsManagerCache(new SecretCacheConfiguration() { CacheItemTTL = 60000 }));
        services.AddSingleton(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}