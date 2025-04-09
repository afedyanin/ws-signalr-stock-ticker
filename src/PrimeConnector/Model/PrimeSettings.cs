namespace PrimeConnector.Model;

public class PrimeSettings
{
    public required string AuthUrl { get; set; }

    public required string WsMarketDataUrl { get; set; }

    public required string ClientId { get; set; }

    public required string ClientSecret { get; set; }

    public required string ClientName { get; set; }

    public string[] Symbols { get; set; } = Array.Empty<string>();
}
