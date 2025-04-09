using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using StockPricesEmulator;
using StockPricesEmulator.Extensions;
using StockPricesEmulator.Model;

namespace WebApp.Hubs;

public class StockTickerHub : Hub
{
    private readonly IStockTickerObservable _stockTickerObservable;

    private readonly ILogger<StockTickerHub> _logger;

    public StockTickerHub(
        IStockTickerObservable stockTickerObservable, 
        ILogger<StockTickerHub> logger)
    {
        _stockTickerObservable = stockTickerObservable;
        _logger = logger;
    }

    public ChannelReader<Stock> StreamStocks()
    {
        _logger.LogInformation("Start streaming stock prices...");
        return _stockTickerObservable.StreamStocks().AsChannelReader(10);
    }
}
