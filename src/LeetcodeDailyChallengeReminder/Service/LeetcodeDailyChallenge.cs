using System.Text.Json;
using Amazon.Lambda.Core;

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
}