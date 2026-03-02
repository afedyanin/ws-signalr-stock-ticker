using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;

namespace Ws.Server.Host.Controllers;

public class WebSocketController : ControllerBase
{
    [Route("/ws/weather")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await SendWeatherUpdates(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
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
