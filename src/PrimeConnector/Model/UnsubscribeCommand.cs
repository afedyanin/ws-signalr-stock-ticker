namespace PrimeConnector.Model
{
    public class UnsubscribeCommand
    {
        public string Cmd => "UNSUBSCRIBE";

        public int CmdId { get; init; }

        public int SubscriptionId { get; init; }
    }
}
