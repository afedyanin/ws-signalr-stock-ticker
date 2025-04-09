namespace PrimeConnector.Model;

public record class StreamMessage
{
    public string? Cmd {  get; set; } 

    public long SubscriptionId { get; set; }

    public string? Status { get; set; } 
    
    public StreamBody? Body { get; set; } 
}
