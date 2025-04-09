using PrimeConnector.Model;

namespace PrimeConnector;

public interface IMarketDataObservable
{
    public IObservable<MarketQuote> StreamMerketData();
    
    public IObservable<SubscribeStatus> StreamSubscribeStatus();

    public IEnumerable<MarketQuote> GetMarketQuotes();

    public IEnumerable<SubscribeStatus> GetSubscriptions();
}
