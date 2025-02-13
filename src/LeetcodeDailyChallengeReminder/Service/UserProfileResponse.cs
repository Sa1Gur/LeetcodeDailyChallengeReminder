namespace LeetcodeDailyChallengeReminder.Service;

public record UserProfileResponse
{
    public required List<QuestionCount> AllQuestionsCount { get; set; }
    public required MatchedUser MatchedUser { get; set; }
    public required List<RecentSubmission> RecentSubmissionList { get; set; }
}
