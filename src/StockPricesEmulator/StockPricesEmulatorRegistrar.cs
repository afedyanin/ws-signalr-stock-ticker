using Microsoft.Extensions.DependencyInjection;

namespace StockPricesEmulator;

public static class StockPricesEmulatorRegistrar
{
    public static IServiceCollection AddStockPricesEmulator(this IServiceCollection services)
    {
        services.AddSingleton<IStockTickerService, StockTickerService>();
        services.AddTransient<IStockSymbolProvider, StockSymbolPorvider>();

        services.AddSingleton<StockTickerSubjectHandler>();
        services.AddSingleton<IStockTickerObservable>(x => x.GetRequiredService<StockTickerSubjectHandler>());
        services.AddSingleton<IStockTickerDataHandler>(x => x.GetRequiredService<StockTickerSubjectHandler>());
        
        return services;
    }
}
