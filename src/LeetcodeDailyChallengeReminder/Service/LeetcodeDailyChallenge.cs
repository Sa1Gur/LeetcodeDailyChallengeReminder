using System.Text.Json;
using Amazon.Lambda.Core;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.SystemTextJson;
using LeetcodeDailyChallengeReminder.Service;

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
