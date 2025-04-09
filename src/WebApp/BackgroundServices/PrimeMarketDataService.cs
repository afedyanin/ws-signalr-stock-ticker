
using PrimeConnector;

namespace WebApp.BackgroundServices;

public class PrimeMarketDataService : BackgroundService
{
    private readonly IMarketDataService _marketDataService;

    public PrimeMarketDataService(
        IMarketDataService marketDataService)
    {
        _marketDataService = marketDataService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // await Task.Delay(10000, stoppingToken);

        await _marketDataService.ExecuteAsync(stoppingToken);
    }
}
