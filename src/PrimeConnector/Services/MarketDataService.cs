using System.Net.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrimeConnector.Extensions;
using PrimeConnector.Factories;
using PrimeConnector.Model;
using Websocket.Client;

namespace PrimeConnector.Services
{
    internal class MarketDataService : IMarketDataService
    {
        private readonly PrimeSettings _primeSettings;

        private readonly ISymbolProvider _symbolProvider;
        private readonly IMarketDataMessageHandler _messageHandler;
        private readonly IServiceProvider _serviceProvider;

        private readonly ILogger<MarketDataService> _logger;
        private readonly ILogger<WebsocketClient> _wsLogger;

        public MarketDataService(
            IServiceProvider serviceProvider,
            IOptions<PrimeSettings> options,
            ISymbolProvider symbolProvider,
            IMarketDataMessageHandler messageHandler,
            ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider;
            _primeSettings = options.Value;
            _messageHandler = messageHandler;
            _symbolProvider = symbolProvider;
            
            _logger = loggerFactory.CreateLogger<MarketDataService>();
            _wsLogger = loggerFactory.CreateLogger<WebsocketClient>();
        }

        public async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            var factory = CreateFactory();

            var merketDataUrl = new Uri(_primeSettings.WsMarketDataUrl);

            using IWebsocketClient client = 
                new WebsocketClient(merketDataUrl, _wsLogger, factory);

            client.Name = _primeSettings.ClientName;
            client.ReconnectTimeout = TimeSpan.FromSeconds(300);
            client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);

            client.ReconnectionHappened.Subscribe(info =>
            {
                _logger.LogInformation("Reconnection happened, type: {type}, url: {url}", info.Type, client.Url);
            });

            client.DisconnectionHappened.Subscribe(info =>
                _logger.LogWarning("Disconnection happened, type: {type}", info.Type));

            client.MessageReceived.Subscribe(async msg =>
            {
                var model = MessageFactory.CreateMarketDataMessage(msg.Text);
                
                if (model != null)
                {
                    var task = model switch
                    {
                        StreamMessage streamMsg =>
                            _messageHandler.Handle(streamMsg, cancellationToken),
                        SubscribeStatus subStatus =>
                            _messageHandler.Handle(subStatus, cancellationToken),
                        _ => Task.CompletedTask
                    };

                    await task;
                }
            });

            _logger.LogInformation("Starting...");

            await client.Start();

            _logger.LogInformation("Started.");

            foreach (var symbol in _symbolProvider.GetSymbols())
            {
                client.Send(new SubscribeCommand(symbol).ToJson());
            }

            cancellationToken.WaitHandle.WaitOne();
        }

        private Func<Uri, CancellationToken, Task<WebSocket>> CreateFactory()
        {
            var factory = new Func<Uri, CancellationToken, Task<WebSocket>>(
                async (uri, cancellationToken) =>
                {
                    var client = new ClientWebSocket
                    {
                        Options =
                        {
                            KeepAliveInterval = TimeSpan.FromSeconds(20),
                            // Proxy = ...
                            // ClientCertificates = ...
                        }
                    };

                    using var scope = _serviceProvider.CreateScope();
                    var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
                    var token = await authService.Authenticate();
                    _logger.LogDebug($"Access token: {token!.AccessToken}");
                    client.Options.SetRequestHeader("Authorization", $"Bearer {token!.AccessToken}");

                    await client.ConnectAsync(uri, cancellationToken);

                    return client;
                });
            
            return factory;
        }
    }
}
