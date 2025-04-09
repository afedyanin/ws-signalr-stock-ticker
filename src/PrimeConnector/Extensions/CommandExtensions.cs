using PrimeConnector.Model;
using System.Text.Json;

namespace PrimeConnector.Extensions
{
    public static class CommandExtensions
    {
        private static readonly JsonSerializerOptions _serializeOptions =
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };

        public static string ToJson(this SubscribeCommand command)
            => JsonSerializer.Serialize(command, _serializeOptions);
    }
}
