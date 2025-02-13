namespace LeetcodeDailyChallengeReminder.Service;

public record QuestionCount
{
    public required string Difficulty { get; set; }
    public int Count { get; set; }
}
