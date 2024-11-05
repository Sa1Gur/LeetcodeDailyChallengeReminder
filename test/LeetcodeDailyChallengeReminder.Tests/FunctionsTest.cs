using Xunit;
using Amazon.Lambda.TestUtilities;
using Moq;
using Amazon.SecretsManager.Extensions.Caching;
using System.Text.Json;
using System.ComponentModel;


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
    [Description("We expect the test to fail when calling the bot after successfully getting credentials.")]
    public async void TestCredentials()
    {
        TestLambdaContext context = new();

        Functions functions = new(_mockSecretsManagerCache, _jsonSerializerOptions);
        Assert.Equal("Fail to get updates", await functions.Test(context));
    }
}
