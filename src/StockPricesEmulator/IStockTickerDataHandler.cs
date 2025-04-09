using StockPricesEmulator.Model;

namespace StockPricesEmulator
{
    public interface IStockTickerDataHandler
    {
        public Task HandlePriceChange(Stock stock, CancellationToken cancellationToken = default);

        public Task HandleMarketStateChange(MarketState marketState, CancellationToken cancellationToken = default);
        
        public Task HandleReset(CancellationToken cancellationToken = default);
    }
}
