using StockPricesEmulator;

namespace WebApp.BackgroundServices
{
    public class StockTickerBackgroundService : BackgroundService
    {
        private readonly IStockTickerService _stockTickerService;
        private readonly ILogger<StockTickerBackgroundService> _logger;

        public StockTickerBackgroundService(
            IStockTickerService stockTickerService,
            ILogger<StockTickerBackgroundService> logger)
        {
            _stockTickerService = stockTickerService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                await _stockTickerService.OpenMarket();

                while (!stoppingToken.IsCancellationRequested)
                { 
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}", ex);
            }
            finally
            {
                await _stockTickerService.CloseMarket();
            }
        }
    }
}
