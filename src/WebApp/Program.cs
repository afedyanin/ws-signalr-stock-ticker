using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.FluentUI.AspNetCore.Components;
using PrimeConnector;
using StockPricesEmulator;
using WebApp.BackgroundServices;
using WebApp.Components.Infrastructure;
using WebApp.Hubs;

namespace WebApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddPrimeConnector(builder.Configuration);
        builder.Services.AddHostedService<PrimeMarketDataService>();
        
        builder.Services.AddStockPricesEmulator();
        builder.Services.AddHostedService<StockTickerBackgroundService>();
        
        StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

        builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:5000") });

        builder.Services.AddRazorPages();

        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddServerSideBlazor();

        builder.Services.AddFluentUIComponents();
        builder.Services.AddFluentUIDemoServices();

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSignalR(options =>
            options.EnableDetailedErrors = true)
                .AddMessagePackProtocol();

        builder.Services.AddCors(opts => opts.AddDefaultPolicy(bld =>
        {
            bld
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .WithExposedHeaders("*")
            ;
        }));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseWebAssemblyDebugging();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHub<StockTickerHub>("/hubs/stocks");
        app.MapHub<PrimeMarketDataHub>("/hubs/prime");

        app.MapBlazorHub();

        app.MapFallbackToPage("/_Host");

        app.UseAntiforgery();

        app.Run();
    }
}
