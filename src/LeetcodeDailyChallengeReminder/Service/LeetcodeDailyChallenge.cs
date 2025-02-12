using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda.Core;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;

namespace LeetcodeDailyChallenge.LeetcodeDailyChallenge;

public class LeetcodeDailyChallenge
{
    public static async Task<DailyChallengeInfo?> GetLeetcodeDailyChallenge(
        HttpClient client,
        JsonSerializerOptions options,
        ILambdaLogger logger)
    {
        string daily = """
    https://leetcode.com/graphql?query=query
     daily {
        challenge: activeDailyCodingChallengeQuestion {
            date
            link
            question {
                difficulty
                id: questionFrontendId
                title
                slug: titleSlug
                tags: topicTags {
                    name
                    slug
                }
            }
        }
    }
""";

        try
        {
            HttpResponseMessage response = await client.GetAsync(daily);
            response.EnsureSuccessStatusCode();
            DailyChallengeInfo? document = JsonSerializer.Deserialize<DailyChallengeInfo>(
                await response.Content.ReadAsStringAsync(),
                options
            );

            return document;
        }
        catch (HttpRequestException ex)
        {
            logger.LogError($"Error downloading content from {daily}: {ex.Message}");
            return null;
        }
    }

    public static async Task<UserProfileResponse?> GetUserLeetcodeStatus(
        string username,
        JsonSerializerOptions options,
        ILambdaLogger logger)
    {
        string leetCodeGraphQLUrl = "https://leetcode.com/graphql";
var query = @"
query getUserProfile($username: String!) {
    allQuestionsCount {
        difficulty
        count
    }
    matchedUser(username: $username) {
        username
        socialAccounts
        githubUrl
        contributions {
            points
            questionCount
            testcaseCount
        }
        profile {
            realName
            websites
            countryName
            skillTags
            company
            school
            starRating
            aboutMe
            userAvatar
            reputation
            ranking
        }
        submissionCalendar
        submitStats: submitStatsGlobal {
            acSubmissionNum {
                difficulty
                count
                submissions
            }
        }
    }
    recentSubmissionList(username: $username, limit: 20) {
        title
        titleSlug
        timestamp
        statusDisplay
        lang
    }
}";

        using var graphQLClient = new GraphQLHttpClient(leetCodeGraphQLUrl, new SystemTextJsonSerializer());

        GraphQLRequest request = new()
        {
            Query = query,
            Variables = new { username }
        };

        var response = await graphQLClient.SendQueryAsync<UserProfileResponse>(request);

        if (response.Errors != null && response.Errors.Length > 0)
        {
            logger.LogError($"Errors:{string.Join("\n", response.Errors.Select(x => x.ToString()))}");
            return null;
        }

        return response.Data;
    }
}

public class UserProfileResponse
{
    public List<QuestionCount> AllQuestionsCount { get; set; }
    public MatchedUser MatchedUser { get; set; }
    public List<RecentSubmission> RecentSubmissionList { get; set; }
}

public class QuestionCount
{
    public string Difficulty { get; set; }
    public int Count { get; set; }
}

public class MatchedUser
{
    public string Username { get; set; }
    public List<string> SocialAccounts { get; set; }
    public string GithubUrl { get; set; }
    public Contributions Contributions { get; set; }
    public Profile Profile { get; set; }
    public string SubmissionCalendar { get; set; }
    public SubmitStats SubmitStats { get; set; }
}

public class Contributions
{
    public int Points { get; set; }
    public int QuestionCount { get; set; }
    public int TestcaseCount { get; set; }
}

public class Profile
{
    public string RealName { get; set; }
    public List<string> Websites { get; set; }
    public string CountryName { get; set; }
    public List<string> SkillTags { get; set; }
    public string Company { get; set; }
    public string School { get; set; }
    public double StarRating { get; set; }
    public string AboutMe { get; set; }
    public string UserAvatar { get; set; }
    public int Reputation { get; set; }
    public int Ranking { get; set; }
}

public class SubmitStats
{
    public List<AcSubmissionNum> AcSubmissionNum { get; set; }
}

public class AcSubmissionNum
{
    public string Difficulty { get; set; }
    public int Count { get; set; }
    public int Submissions { get; set; }
}

public class RecentSubmission
{
    public string Title { get; set; }
    public string TitleSlug { get; set; }
    [JsonConverter(typeof(UnixTimestampToDateTimeConverter))]
    public DateTime Timestamp { get; set; }
    public string StatusDisplay { get; set; }
    public string Lang { get; set; }
}

public class UnixTimestampToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string stringValue = reader.GetString();
            if (long.TryParse(stringValue, out long unixTimestamp))
            {
                return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            long unixTimestamp = reader.GetInt64();
            return DateTimeOffset.FromUnixTimeSeconds(unixTimestamp).DateTime;
        }

        throw new JsonException("Unable to convert timestamp to DateTime");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        long unixTimestamp = ((DateTimeOffset)value).ToUnixTimeSeconds();
        writer.WriteNumberValue(unixTimestamp);
    }
}
