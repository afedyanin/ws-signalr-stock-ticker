using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using PrimeConnector;
using PrimeConnector.Model;
using StockPricesEmulator.Extensions;

namespace WebApp.Hubs;

public class PrimeMarketDataHub : Hub
{
    private readonly IMarketDataObservable _primeMarketDataObservable;

    private readonly ILogger<PrimeMarketDataHub> _logger;

    public PrimeMarketDataHub(
        IMarketDataObservable primeMarketDataObservable,
        ILogger<PrimeMarketDataHub> logger)
    {
        _primeMarketDataObservable = primeMarketDataObservable;
        _logger = logger;
    }

    public IEnumerable<MarketQuote> GetAllQuotes()
        => _primeMarketDataObservable.GetMarketQuotes();

    public ChannelReader<MarketQuote> StreamMarketData()
    {
        _logger.LogInformation("Start streaming Prime market data ...");
        return _primeMarketDataObservable.StreamMerketData().AsChannelReader(10);
    }

    public IEnumerable<SubscribeStatus> GetAllSubscriptions()
        => _primeMarketDataObservable.GetSubscriptions();

    public ChannelReader<SubscribeStatus> StreamSubscribeStatus()
    {
        _logger.LogInformation("Start streaming Prime market data subscribe status ...");
        return _primeMarketDataObservable.StreamSubscribeStatus().AsChannelReader(10);
    }
}
