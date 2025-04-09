namespace PrimeConnector.Model;

public record class StreamBody
{
    public string? Instrument {  get; set; }

    public StreamData? Data { get; set; }
}
