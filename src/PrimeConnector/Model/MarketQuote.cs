namespace PrimeConnector.Model
{
    public record class MarketQuote
    {
        public string? Instrument { get; set; }
        public DateTime? DateTime { get; set; }
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
        public decimal? OpenInterest { get; set; }
        public decimal? TradesToday { get; set; }
        public decimal? Volume { get; set; }
        public decimal? PrevVol { get; set; }
        public decimal? BidSize { get; set; }
        public decimal? AskSize { get; set; }
        public decimal? LastSize { get; set; }
    }
}
