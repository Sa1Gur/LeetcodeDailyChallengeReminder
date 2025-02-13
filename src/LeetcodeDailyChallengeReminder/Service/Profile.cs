namespace LeetcodeDailyChallengeReminder.Service;

public record Profile
{
    public required string RealName { get; set; }
    public required List<string> Websites { get; set; }
    public string? CountryName { get; set; }
    public required List<string> SkillTags { get; set; }
    public string? Company { get; set; }
    public string? School { get; set; }
    public double StarRating { get; set; }
    public string? AboutMe { get; set; }
    public string? UserAvatar { get; set; }
    public int Reputation { get; set; }
    public int Ranking { get; set; }
}
