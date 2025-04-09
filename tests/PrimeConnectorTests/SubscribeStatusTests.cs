using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PrimeConnector.Extensions;
using PrimeConnector.Model;

namespace PrimeConnectorTests;

public class SubscribeStatusTests
{
    [TestCase("Data/Subscribe001.json")]
    [TestCase("Data/Subscribe002.json")]
    [TestCase("Data/Subscribe003.json")]
    public void CanDeserializeMessage(string fileName)
    {
        var text = File.ReadAllText(fileName);
        var message = JsonSerializer.Deserialize<SubscribeStatus>(text, JsonSerializerDefaultOptions.Instance);

        Assert.That(message, Is.Not.Null);
        Assert.That(message.Body, Is.Not.Null);

        Console.WriteLine(message);
    }

}
