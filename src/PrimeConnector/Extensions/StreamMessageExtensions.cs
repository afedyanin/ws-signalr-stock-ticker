using PrimeConnector.Model;

namespace PrimeConnector.Extensions
{
    internal static class StreamMessageExtensions
    {
        public static MarketQuote? ToQuote(this StreamMessage? msg)
        {
            if (msg == null || 
                !msg.IsSucess() || 
                msg.Body == null ||
                msg.Body.Data == null)
            {
                return null;
            }

            var data = msg.Body.Data;
            return new MarketQuote
            {
                Instrument = msg.Body.Instrument,
                DateTime = data.DateTime,
                Bid = data.Bid,
                Ask = data.Ask,
                Mid = data.Mid,
                Last = data.Last,
                Open = data.Open,
                High = data.High,
                Low = data.Low,
                Close = data.Close,
                NetChange = data.NetChange,
                PercentChange = data.PercentChange,
                OpenInterest = data.OpenInterest,
                TradesToday = data.TradesToday,
                Volume = data.Volume,
                PrevVol = data.PrevVol,
                BidSize = data.BidSize,
                AskSize = data.AskSize,
                LastSize = data.LastSize,
            };
        }


        private static bool IsSucess(this StreamMessage msg)
            => msg.Status != null && 
               msg.Status.Equals("SUCCESS", StringComparison.OrdinalIgnoreCase);
    }
}
