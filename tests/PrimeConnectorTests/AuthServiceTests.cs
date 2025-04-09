using Microsoft.Extensions.Options;
using PrimeConnector.Model;
using PrimeConnector.Services;

namespace PrimeConnectorTests;

public class AuthServiceTests
{
    private static readonly PrimeSettings _primeSettings =
        new PrimeSettings
        {
            AuthUrl = "http://services.prime-it.pro:9050/auth/token",
            WsMarketDataUrl = "wss://services.prime-it.pro/ws/market-data",
            ClientId = "",
            ClientSecret = "",
            ClientName = "Test"
        };

    [Test]
    public async Task CanGetAuthToken()
    {
        var httpClient = new HttpClient();
        var options = Options.Create(_primeSettings);
        var service = new AuthService(httpClient, options);
        var token = await service.Authenticate();

        Assert.That(token, Is.Not.Null);
        Console.WriteLine(token.AccessToken);
    }
}
