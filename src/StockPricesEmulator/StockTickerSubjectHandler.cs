using StockPricesEmulator.Model;
using System.Reactive.Subjects;

namespace StockPricesEmulator;

public class StockTickerSubjectHandler : IStockTickerDataHandler, IStockTickerObservable
{
    private readonly Subject<MarketState> _marketStateSubject;
    private readonly Subject<Stock> _stockSubject;

    public IObservable<Stock> StreamStocks() => _stockSubject;

    public IObservable<MarketState> StreamMarketState() => _marketStateSubject;

    public StockTickerSubjectHandler()
    {
        _marketStateSubject = new Subject<MarketState>();
        _stockSubject = new Subject<Stock>();
    }

    public Task HandleMarketStateChange(MarketState marketState, CancellationToken cancellationToken = default)
    {
        _marketStateSubject.OnNext(marketState);
        return Task.CompletedTask;
    }

    public Task HandlePriceChange(Stock stock, CancellationToken cancellationToken = default)
    {
        _stockSubject.OnNext(stock);
        return Task.CompletedTask;
    }

    public Task HandleReset(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
