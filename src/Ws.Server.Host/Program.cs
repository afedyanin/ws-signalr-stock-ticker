using System.Net.WebSockets;

namespace Ws.Server.Host;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.UseWebSockets();

        app.Map("/ws/weather", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await SendWeatherUpdates(webSocket);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        });

        await app.RunAsync();
    }

    private static async Task SendWeatherUpdates(WebSocket webSocket)
    {
        var summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        while (webSocket.State == WebSocketState.Open)
        {
            // Generate random weather data
            var forecast = new
            {
                Date = DateTime.UtcNow.ToShortDateString(),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            };

            var message = System.Text.Json.JsonSerializer.Serialize(forecast);
            var bytes = System.Text.Encoding.UTF8.GetBytes(message);

            Console.WriteLine("Push weather data to client...");

            // Push data to client
            await webSocket.SendAsync(
                new ArraySegment<byte>(bytes),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            await Task.Delay(3000); // Send updates every 3 seconds
        }
    }
}
