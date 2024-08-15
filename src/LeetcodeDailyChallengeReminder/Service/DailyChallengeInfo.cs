namespace LeetcodeDailyChallenge.LeetcodeDailyChallenge;

public record Tag
{
    public required string Name { get; set; }
    public required string Slug { get; set; }
}

public record Question
{
    public required string Difficulty { get; set; }
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Slug { get; set; }
    public required List<Tag> Tags { get; set; }
}

public record Challenge
{
    public required string Date { get; set; }
    public required string Link { get; set; }
    public required Question Question { get; set; }
}

public record Data
{
    public required Challenge Challenge { get; set; }
}

public record DailyChallengeInfo
{
    public required Data Data { get; set; }
}