using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeetcodeDailyChallengeReminder.Service;

public class UnixTimestampToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            string? stringValue = reader.GetString();
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
