using StockPricesEmulator.Model;

namespace StockPricesEmulator
{
    public interface IStockSymbolProvider
    {
        public IEnumerable<Stock> Stocks { get; }
    }
}
