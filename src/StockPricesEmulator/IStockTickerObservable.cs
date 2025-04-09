using StockPricesEmulator.Model;

namespace StockPricesEmulator;

public interface IStockTickerObservable
{
    public IObservable<Stock> StreamStocks();

    public IObservable<MarketState> StreamMarketState();
}
