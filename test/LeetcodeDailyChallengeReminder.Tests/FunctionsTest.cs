using Xunit;
using Amazon.Lambda.TestUtilities;
using Moq;
using Amazon.SecretsManager.Extensions.Caching;
using System.Text.Json;


namespace LambdaAnnotations.Tests;

public class FunctionsTest
{
    private readonly ISecretsManagerCache _mockSecretsManagerCache;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    private readonly string token = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
    private readonly int chatId = -123;

    public FunctionsTest()
    {
        Mock<ISecretsManagerCache> mock = new();
        mock
            .Setup(m => m.GetSecretString(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync($"{{\"token\":\"{token}\",\"chatId\":\"{chatId}\"}}");
        _mockSecretsManagerCache = mock.Object;
    }

    [Fact]
    public async void TestCredentials()
    {
        TestLambdaContext context = new();

        Functions functions = new(_mockSecretsManagerCache, _jsonSerializerOptions);
        Assert.Equal(token + " " + chatId, await functions.Test(context));
    }

    [Fact]
    public async void TestGetSecret()
    {
        HttpClient client = new();
        string getUpdatesUrl = $"https://api.telegram.org/.../getUpdates";
        var updates = await client.GetAsync(getUpdatesUrl);
        TelegramUpdate? update = JsonSerializer.Deserialize<TelegramUpdate>(
            await updates.Content.ReadAsStringAsync(), _jsonSerializerOptions
        );
        List<DateTime> dates = [];
        foreach (var result in update?.Result ?? [])
            dates.Add(DateTimeOffset.FromUnixTimeSeconds(result.Message.Date).Date);

        if (update is {})
            Console.WriteLine($"Recent Telegram messages {string.Join(" ", update.Result)}");
    }
}
