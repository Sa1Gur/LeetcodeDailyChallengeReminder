namespace LeetcodeDailyChallengeReminder.Service;

public record AcSubmissionNum
{
    public required string Difficulty { get; set; }
    public int Count { get; set; }
    public int Submissions { get; set; }
}
