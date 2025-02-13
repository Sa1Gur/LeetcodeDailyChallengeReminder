namespace LeetcodeDailyChallengeReminder.Service;

public record MatchedUser
{
    public required string Username { get; set; }
    public required List<string> SocialAccounts { get; set; }
    public string? GithubUrl { get; set; }
    public required Contributions Contributions { get; set; }
    public required Profile Profile { get; set; }
    public string? SubmissionCalendar { get; set; }
    public required SubmitStats SubmitStats { get; set; }
}
