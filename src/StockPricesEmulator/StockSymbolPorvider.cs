
using StockPricesEmulator.Model;

namespace StockPricesEmulator;
internal class StockSymbolPorvider : IStockSymbolProvider
{
    public IEnumerable<Stock> Stocks =>
    [
        new("MSFT", 107.56m),
        new("AAPL", 215.49m),
        new("GOOG", 1221.16m)
    ];
}
