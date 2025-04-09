using System.Text.Json;

namespace PrimeConnector.Extensions
{
    public static class JsonSerializerDefaultOptions
    {
        public static JsonSerializerOptions Instance { get; }

        static JsonSerializerDefaultOptions()
        {
            Instance = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            Instance.Converters.Add(new DateTimeConverterUsingTicks());
        }
    }
}
