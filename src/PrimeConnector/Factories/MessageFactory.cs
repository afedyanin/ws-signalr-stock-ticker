using System.Text.Json;
using PrimeConnector.Model;
using PrimeConnector.Extensions;

namespace PrimeConnector.Factories
{
    internal static class MessageFactory
    {
        public static object? CreateMarketDataMessage(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            var doc = JsonDocument.Parse(text);
            var cmd = GetCommand(doc);

            if (string.IsNullOrEmpty(cmd))
            {
                return null;
            }

            return cmd switch
            {
                "STREAM" => CreateStreamMessage(text),
                "SUBSCRIBE" => CreateSubscribeMessage(text),
                _ => null
            };
        }

        private static StreamMessage? CreateStreamMessage(string text)
            => JsonSerializer.Deserialize<StreamMessage>(text, JsonSerializerDefaultOptions.Instance);

        private static SubscribeStatus? CreateSubscribeMessage(string text)
            => JsonSerializer.Deserialize<SubscribeStatus>(text, JsonSerializerDefaultOptions.Instance);

        private static string? GetCommand(JsonDocument doc)
            => doc.RootElement.TryGetProperty("cmd", out var cmdElement) ?
                cmdElement.GetString() : null;

    }
}
