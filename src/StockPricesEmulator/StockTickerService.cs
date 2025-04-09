using Microsoft.Extensions.Logging;
using StockPricesEmulator.Model;
using System.Collections.Concurrent;

namespace StockPricesEmulator;

public class StockTickerService : IStockTickerService
{
    private readonly ConcurrentDictionary<string, Stock> _stocks = new ConcurrentDictionary<string, Stock>();

    private readonly IStockTickerDataHandler _changeHandler;
    private readonly IStockSymbolProvider _symbolProvider;

    private readonly SemaphoreSlim _marketStateLock = new SemaphoreSlim(1, 1);
    private readonly SemaphoreSlim _updateStockPricesLock = new SemaphoreSlim(1, 1);

    // Stock can go up or down by a percentage of this factor on each change
    private readonly double _rangePercent = 0.02;

    private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(250);
    private readonly Random _updateOrNotRandom = new Random();

    private Timer? _timer;
    private volatile bool _updatingStockPrices;
    private volatile MarketState _marketState;

    private readonly ILogger<StockTickerService> _logger;

    public MarketState MarketState
    {
        get { return _marketState; }
        private set { _marketState = value; }
    }


    public StockTickerService(
        IStockSymbolProvider symbolProvider,
        IStockTickerDataHandler changeHandler,
        ILogger<StockTickerService> logger)
    {
        _changeHandler = changeHandler;
        _symbolProvider = symbolProvider;
        _logger = logger;

        LoadDefaultStocks();
    }

    public async Task OpenMarket()
    {
        await _marketStateLock.WaitAsync();
        try
        {
            if (MarketState != MarketState.Open)
            {
                _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

                MarketState = MarketState.Open;

                _logger.LogInformation("OpenMarket command received.");

                await _changeHandler.HandleMarketStateChange(MarketState.Open);
            }
        }
        finally
        {
            _marketStateLock.Release();
        }
    }

    public async Task CloseMarket()
    {
        await _marketStateLock.WaitAsync();
        try
        {
            if (MarketState == MarketState.Open)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }

                MarketState = MarketState.Closed;

                _logger.LogInformation("CloseMarket command received.");

                await _changeHandler.HandleMarketStateChange(MarketState.Closed);
            }
        }
        finally
        {
            _marketStateLock.Release();
        }
    }

    public async Task Reset()
    {
        await _marketStateLock.WaitAsync();

        try
        {
            if (MarketState != MarketState.Closed)
            {
                throw new InvalidOperationException("Market must be closed before it can be reset.");
            }

            _logger.LogInformation("Reset command received.");

            LoadDefaultStocks();
            await _changeHandler.HandleReset();
        }
        finally
        {
            _marketStateLock.Release();
        }
    }

    private void LoadDefaultStocks()
    {
        _stocks.Clear();

        foreach (var stock in _symbolProvider.Stocks)
        {
            _stocks[stock.Symbol] = stock;
        }
    }

    private async void UpdateStockPrices(object? state)
    {
        // This function must be re-entrant as it's running as a timer interval handler
        await _updateStockPricesLock.WaitAsync();
        try
        {
            if (!_updatingStockPrices)
            {
                _updatingStockPrices = true;

                foreach (var stock in _stocks.Values)
                {
                    TryUpdateStockPrice(stock);

                    _logger.LogDebug($"Price updated for {stock.Symbol}. New price={stock.Price}");

                    await _changeHandler.HandlePriceChange(stock);
                }

                _updatingStockPrices = false;
            }
        }
        finally
        {
            _updateStockPricesLock.Release();
        }
    }

    private bool TryUpdateStockPrice(Stock stock)
    {
        // Randomly choose whether to udpate this stock or not
        var r = _updateOrNotRandom.NextDouble();
        if (r > 0.5)
        {
            return false;
        }

        // Update the stock price by a random factor of the range percent
        var random = new Random((int)Math.Floor(stock.Price));
        var percentChange = random.NextDouble() * _rangePercent;
        var pos = random.NextDouble() > 0.51;
        var change = Math.Round(stock.Price * (decimal)percentChange, 2);
        change = pos ? change : -change;

        stock.Price += change;
        return true;
    }
}
