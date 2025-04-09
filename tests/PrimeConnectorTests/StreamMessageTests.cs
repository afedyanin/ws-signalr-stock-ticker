using System.Text.Json;
using PrimeConnector.Extensions;
using PrimeConnector.Model;

namespace PrimeConnectorTests;

public class StreamMessageTests
{
    [TestCase("Data/StreamMessage001.json")]
    [TestCase("Data/StreamMessage002.json")]
    [TestCase("Data/StreamMessage003.json")]
    [TestCase("Data/StreamMessage004.json")]
    public void CanDeserializeMessage(string fileName)
    {
        var text = File.ReadAllText(fileName);
        var message = JsonSerializer.Deserialize<StreamMessage>(text, JsonSerializerDefaultOptions.Instance);

        Assert.That(message, Is.Not.Null);
        Assert.That(message.Body, Is.Not.Null);
        Assert.That(message.Body.Data, Is.Not.Null);
        
        Console.WriteLine(message);
    }

    [TestCase("Data/StreamData001.json")]
    public void CanDeserializeStreamData(string fileName)
    {
        var text = File.ReadAllText(fileName);
        var message = JsonSerializer.Deserialize<StreamData>(text, JsonSerializerDefaultOptions.Instance);

        Assert.That(message, Is.Not.Null);
        Console.WriteLine(message);
    }

    [TestCase(1743638400000)]
    [TestCase(1743781484056)]
    public void CanConvertTicksToDateTime(long ticks)
    {
        TimeSpan time = TimeSpan.FromMilliseconds(ticks);
        var date = new DateTime(1970, 1, 1) + time; 
        Console.WriteLine(date.ToString("O"));
    }

    [TestCase("Data/StreamMessage001.json")]
    [TestCase("Data/Subscribe001.json")]
    [TestCase("Data/SubscribeError.json")]
    [TestCase("Data/BadRequest.json")]
    public void CanParseJson(string fileName)
    {
        var text = File.ReadAllText(fileName);
        var doc = JsonDocument.Parse(text);

        doc.RootElement.TryGetProperty("cmd", out var cmd);
        doc.RootElement.TryGetProperty("status", out var status);

        Console.WriteLine($"cmd={cmd} status={status}");
    }

}
