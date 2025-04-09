using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeConnector.Model;
using PrimeConnector.Services;

namespace PrimeConnector
{
    public static class PrimeConnectorRegistrar
    {
        public static IServiceCollection AddPrimeConnector(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-9.0
            services.AddHttpClient<AuthService>();

            services.AddTransient<ISymbolProvider, ConfigSymbolProvider>();
            services.AddSingleton<IMarketDataService, MarketDataService>();

            services.Configure<PrimeSettings>(
                configuration.GetSection(nameof(PrimeSettings)));

            services.AddSingleton<MarketDataSubjectHandler>();
            services.AddSingleton<IMarketDataMessageHandler>(x => x.GetRequiredService<MarketDataSubjectHandler>());
            services.AddSingleton<IMarketDataObservable>(x => x.GetRequiredService<MarketDataSubjectHandler>());

            return services;
        }
    }
}
