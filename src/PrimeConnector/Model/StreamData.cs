using System.Text.Json.Serialization;

namespace PrimeConnector.Model;

public record class StreamData
{
    public DateTime? DateTime {  get; set; }
    public decimal? Bid { get; set; }
    public decimal? Ask { get; set; }
    public decimal? Mid { get; set; }
    public decimal? Last { get; set; }
    public decimal? Open { get; set; }
    public decimal? High { get; set; }
    public decimal? Low { get; set; }
    public decimal? Close { get; set; }
    public decimal? NetChange { get; set; }
    public decimal? PercentChange { get; set; }
    public decimal? Vwap { get; set; }
    public decimal? PrimAct1 { get; set; }
    public decimal? PrimAct2 { get; set; }
    public decimal? SecAct1 { get; set; }
    public decimal? SecAct2 { get; set; }
    public decimal? GenVal1 { get; set; }
    public decimal? GenVal2 { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? MaturityDate { get; set; }
    public decimal? Yield {  get; set; }
    public decimal? OpenInterest { get; set; }
    public decimal? TradesToday { get; set; }
    public decimal? Volume { get; set; }
    public decimal? PrevVol { get; set; }
    public decimal? BidSize { get; set; }
    public decimal? AskSize { get; set; }
    public decimal? LastSize { get; set; }

    [JsonPropertyName("52wkLow")]
    public decimal? X52wkLow { get; set; }

    [JsonPropertyName("52wkHigh")]
    public decimal? X52wkHigh { get; set; }

    [JsonPropertyName("52wkLowDate")]
    public DateTime? X52wkLowDate { get; set; }

    [JsonPropertyName("52wkHighDate")]
    public DateTime? X52wkHighDate { get; set; }
    public decimal? Settle {  get; set; }
    public DateTime? SettlementDate { get; set; }
}
