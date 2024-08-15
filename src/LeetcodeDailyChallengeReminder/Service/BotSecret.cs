namespace LeetcodeDailyChallenge.Service;

public record BotSecret
{
    public required string Token { get; init; }
    public required string ChatId { get; init; }
}