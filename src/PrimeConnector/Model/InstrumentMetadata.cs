namespace PrimeConnector.Model;

public record class InstrumentMetadata
{
    public string? Isin { get; set; }

    public string? InstrumentName { get; set; }

    public string? Ticker { get; set; }

    public string? ExchangeName { get; set; }

    public string? Currency { get; set; }
}
