namespace StockPricesEmulator;

public interface IStockTickerService
{
    public Task OpenMarket();
    
    public Task CloseMarket();

    public Task Reset();
}
