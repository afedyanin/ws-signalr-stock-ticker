using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace Ws.Client.ConsoleApp;

internal static class Program
{
    static async Task Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        using var client = new ClientWebSocket();

        var serverUri = new Uri("wss://localhost:7164/ws/weather");
        await client.ConnectAsync(serverUri, cts.Token);

        Console.WriteLine("Connected to Weather Stream!");

        var buffer = new byte[1024 * 4];

        while (client.State == WebSocketState.Open)
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", cts.Token);
            }
            else
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var forecast = JsonSerializer.Deserialize<WeatherForecast>(message);

                if (forecast != null)
                {
                    Console.WriteLine($"[{forecast.Date}] Temp: {forecast.TemperatureC}°C | {forecast.Summary}");
                }
            }
        }
    }
}
