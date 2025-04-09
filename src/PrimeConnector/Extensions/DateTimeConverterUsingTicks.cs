using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PrimeConnector.Extensions
{
    public class DateTimeConverterUsingTicks : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            var ticks = reader.GetInt64();
            TimeSpan time = TimeSpan.FromMilliseconds(ticks);
            var date = new DateTime(1970, 1, 1) + time;
            return date;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Ticks.ToString());
        }
    }
}
