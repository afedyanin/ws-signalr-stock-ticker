namespace PrimeConnector.Model;

public record class SubscribeStatus
{
    public string? Cmd { get; set; }

    public long CmdId { get; set; }

    public long SubscriptionId { get; init; }

    public string? Status { get; set; }

    public SubscribeStatusBody? Body { get; set; }
}
