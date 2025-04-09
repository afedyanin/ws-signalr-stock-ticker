using System;
namespace PrimeConnector.Model;

public record class SubscribeStatusBody
{
    public string? Instrument { get; set; }

    public InstrumentMetadata? Metadata { get; set; }
}
