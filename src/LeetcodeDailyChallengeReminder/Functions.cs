using Amazon.Lambda.Core;
using Amazon.Lambda.Annotations;
using Amazon.Lambda.Annotations.APIGateway;
using Amazon.SecretsManager.Extensions.Caching;
using System.Text.Json;
using System.Collections.ObjectModel;
using LeetcodeDailyChallenge.LeetcodeDailyChallenge;
using LeetcodeDailyChallenge.Service;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace LambdaAnnotations;

public class Functions(ISecretsManagerCache secretsManagerCache, JsonSerializerOptions jsonSerializerOptions)
{
    private static readonly ReadOnlyCollection<string> s_phrasesToSayNo = new List<string>
    {
        // English phrases
        "no",
        "no, thank you",
        "of course not",
        "not in any case",
        "not for anything",
        "regretfully, no",
        "well, no",
        "not yet",
        "nah",
        "it seems not",
        "not sure",
        "not now",
        "naw",
        "no way",
        "not at all",
        "not at this time",
        "not right now",
        "not on your watch",

        // Russian phrases
        "нет",
        "нет, спасибо",
        "конечно нет",
        "ни в коем случае",
        "ни за что",
        "к сожалению, нет",
        "да нет",
        "ещё нет",
        "не",
        "не похоже",
        "не уверен",
        "не сейчас",
        "не-а",
        "ну нет",
        "нету",
        "не в этот раз",
        "не сегодня",
        "не сейчас",
        "не на этом круге"
    }.AsReadOnly();
    private static readonly ReadOnlyCollection<string> s_phrasesToSayDone = new List<string>
    {
        // English phrases
        "done",
        "all done",
        "finished",
        "completed",
        "it's done",
        "that's it",
        "mission accomplished",
        "all set",
        "wrapped up",
        "job done",
        "task completed",
        "it's finished",
        "we're done",
        "that's a wrap",
        "yeah",

        // Russian phrases
        "готово",
        "все готово",
        "закончено",
        "выполнено",
        "сделано",
        "вот и все",
        "задание выполнено",
        "все в порядке",
        "завершено",
        "работа сделана",
        "задача выполнена",
        "это закончено",
        "мы закончили",
        "это все",
        "работа завершена",
        "задание выполнено",
        "задача сделана",
        "это закончилось",
        "работа завершена",
        "ага"
    }.AsReadOnly();

    private static readonly HttpClient s_client = new();

    private readonly ISecretsManagerCache _secretsManagerCache = secretsManagerCache;

    [LambdaFunction()]
    [HttpApi(LambdaHttpMethod.Get, "/test")]
    public async Task<string> Test(ILambdaContext context)
    {
        context.Logger.LogInformation("Starting check");

        DailyChallengeInfo? daily = await LeetcodeDailyChallenge.LeetcodeDailyChallenge.LeetcodeDailyChallenge.GetLeetcodeDailyChallenge(s_client, jsonSerializerOptions, context.Logger);
        if (daily is null) return "Fail to get daily challenge";
        context.Logger.LogInformation("Daily challenge received {daily.Data}");

        string creds = await _secretsManagerCache.GetSecretString("LeetcodeDailyChallengeBot");
        context.Logger.LogInformation("Creds received");
        BotSecret? document = JsonSerializer.Deserialize<BotSecret>(creds, jsonSerializerOptions);
        if (document is null) return "Fail to deserialize creds";
        context.Logger.LogInformation("Creds deserialized succesfully");

        string getUpdatesUrl = $"https://api.telegram.org/bot{document.Token}/getUpdates";
        HttpResponseMessage updates = await s_client.GetAsync(getUpdatesUrl);
        context.Logger.LogInformation($"Daily challenge response success status {updates.IsSuccessStatusCode}, content {updates.Content}");
        if (!updates.IsSuccessStatusCode) return "Fail to get updates";

        TelegramUpdate? update = JsonSerializer.Deserialize<TelegramUpdate>(await updates.Content.ReadAsStringAsync(), jsonSerializerOptions);
        if (update is null) return "Fail to deserialize update";
        if (!update.Ok) return "Update status not ok";

        string debugUpdateResult = string.Join(" ", update.Result);
        context.Logger.LogInformation($"Recent Telegram messages {debugUpdateResult}");

        foreach (var result in update.Result)
        {
            if (result.Message.From.Username == "Guriy_Samarin" &&
                NotToday(result) &&
                DateTimeOffset.FromUnixTimeSeconds(result.Message.Date).Date == DateTime.UtcNow.Date) return "Not today!";
        }

        context.Logger.LogInformation("Posting on Telegram");

        string message = $"Видел сегодняшнюю задачу \"{daily.Data.Challenge.Question.Title}\" ? \n Будешь решать https://leetcode.com{daily.Data.Challenge.Link} ?";
        string url = $"https://api.telegram.org/bot{document.Token}/sendMessage?chat_id={document.ChatId}&text={message}";
        var response = await s_client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        context.Logger.LogInformation($"Daily challenge checked successfully {daily.Data.Challenge.Question.Title}");

        return daily.Data.Challenge.Question.Title;
    }

    private static bool NotToday(TelegramUpdate.MessageUpdate result) =>
        (result.Message.Text is {}) &&
        (s_phrasesToSayDone.Contains(result.Message.Text?.ToLower()) ||
        s_phrasesToSayNo.Contains(result.Message.Text?.ToLower()));


    [LambdaFunction()]
    [HttpApi(LambdaHttpMethod.Get, "/")]
    public string Default()
    {
        var docs = @"LeetcodeDailyChallenge home
        use /test to initiate the daily challenge check
";
        return docs;
    }
}
