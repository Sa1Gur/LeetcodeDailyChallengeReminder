using System.Text.Json.Serialization;

namespace LeetcodeDailyChallengeReminder.Service;

public record RecentSubmission
{
    public required string Title { get; set; }
    public required string TitleSlug { get; set; }
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime Timestamp { get; set; }
    public required string StatusDisplay { get; set; }
    public required string Lang { get; set; }
}
