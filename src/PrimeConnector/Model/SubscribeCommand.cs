namespace PrimeConnector.Model;

public class SubscribeCommand
{
    public string Cmd => "SUBSCRIBE";

    public long CmdId { get; private set; }  

    public string Instrument { get; private set; }

    public SubscribeCommand(string instrument, long? cmdId = null)
    {
        Instrument = instrument;
        CmdId = cmdId ?? DateTime.UtcNow.Ticks;
    }
}
