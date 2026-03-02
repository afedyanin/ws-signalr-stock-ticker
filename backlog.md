## 2026-03-02

- https://medium.com/@bhargavkoya56/building-production-ready-websocket-servers-in-c-asp-net-core-927b737f14cc
- https://github.com/slthomason/StartupHakk/blob/main/92_Implementing_WebSocket_Client_Server_ASPNETCORE/WS-Server-Multiple.cs
- https://github.com/dotnet/AspNetCore.Docs/tree/main/aspnetcore/fundamentals/websockets/samples
- https://medium.com/bina-nusantara-it-division/implementing-websocket-client-and-server-on-asp-net-core-6-0-c-4fbda11dbceb
- 
### Handle client disconnects

The server isn't automatically informed when the client disconnects due to loss of connectivity. 
The server receives a disconnect message only if the client sends it, which can't be done if the internet connection is lost. 
If you want to take some action when that happens, set a timeout after nothing is received from the client within a certain time window.

If the client isn't always sending messages and you don't want to time out just because the connection goes idle, 
have the client use a timer to send a ping message every X seconds. 
On the server, if a message hasn't arrived within 2*X seconds after the previous one, terminate the connection and report that the client disconnected. 
Wait for twice the expected time interval to leave extra time for network delays that might hold up the ping message.


### Background service

If you're using a background service to write data to a WebSocket, make sure you keep the middleware pipeline running. 
Do this by using a TaskCompletionSource<TResult>. 
Pass the TaskCompletionSource to your background service and have it call TrySetResult when you finish with the WebSocket. 
Then await the Task property during the request, as shown in the following example:

``` csharp

app.Run(async (context) =>
{
    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var socketFinishedTcs = new TaskCompletionSource<object>();

    BackgroundSocketProcessor.AddSocket(webSocket, socketFinishedTcs);

    await socketFinishedTcs.Task;
});

```

### Full duplex

```
private async Task HandleFullDuplex(WebSocket webSocket)
{
    // 1. Task to handle incoming messages (Receive Loop)
    var receiveTask = Task.Run(async () => {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close) break;
            
            // Process incoming message here...
        }
    });

    // 2. Task to handle outgoing messages (Send Loop / Background Push)
    var sendTask = Task.Run(async () => {
        while (webSocket.State == WebSocketState.Open)
        {
            // Example: Push server-side data every 5 seconds independently
            var message = Encoding.UTF8.GetBytes($"Server Time: {DateTime.Now}");
            await webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);
            await Task.Delay(5000); 
        }
    });

    // Wait for either loop to fail or close
    await Task.WhenAny(receiveTask, sendTask);
    
    if (webSocket.State != WebSocketState.Closed)
    {
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }
}

```

## 2026-02-28

- https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-10.0
- https://github.com/sta/websocket-sharp
- https://www.luisllamas.es/en/csharp-websockets/


