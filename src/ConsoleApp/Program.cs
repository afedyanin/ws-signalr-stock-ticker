using System.Net.WebSockets;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PrimeConnector.Extensions;
using PrimeConnector.Factories;
using PrimeConnector.Model;
using PrimeConnector.Services;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using Websocket.Client;

namespace ConsoleApp
{
    internal class Program
    {
        private static readonly ManualResetEvent ExitEvent = new(false);

        private static readonly PrimeSettings _primeSettings =
            new PrimeSettings
            {
                AuthUrl = "http://services.prime-it.pro:9050/auth/token",
                WsMarketDataUrl = "ws://services.prime-it.pro:9050/ws/market-data",
                ClientId = "",
                ClientSecret = "",
                ClientName = "TEST"
            };

        private async static Task Main()
        {
            var logFactory = InitLogging();

            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnProcessExit;
            AssemblyLoadContext.Default.Unloading += DefaultOnUnloading;
            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            Console.WriteLine("|=======================|");
            Console.WriteLine("|    WEBSOCKET CLIENT   |");
            Console.WriteLine("|=======================|");
            Console.WriteLine();

            Log.Debug("====================================");
            Log.Debug("              STARTING              ");
            Log.Debug("====================================");

            var logger = logFactory.CreateLogger<WebsocketClient>();

            var httpClient = new HttpClient();
            var options = Options.Create(_primeSettings);
            var service = new AuthService(httpClient, options);
            var token = await service.Authenticate();

            var factory = new Func<ClientWebSocket>(() =>
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

                Log.Information($"Access token: {token!.AccessToken}");
                client.Options.SetRequestHeader("Authorization", $"Bearer {token!.AccessToken}");
                
                return client;
            });

            var url = new Uri(_primeSettings.WsMarketDataUrl);

            using (IWebsocketClient client = new WebsocketClient(url, logger, factory))
            {
                client.Name = "Test";
                client.ReconnectTimeout = TimeSpan.FromSeconds(30);
                client.ErrorReconnectTimeout = TimeSpan.FromSeconds(30);
                
                client.ReconnectionHappened.Subscribe(info =>
                {
                    Log.Information("Reconnection happened, type: {type}, url: {url}", info.Type, client.Url);
                });
                
                client.DisconnectionHappened.Subscribe(info =>
                    Log.Warning("Disconnection happened, type: {type}", info.Type));

                client.MessageReceived.Subscribe(msg =>
                {
                    var model = MessageFactory.CreateMarketDataMessage(msg.Text);

                    if (model != null)
                    {
                        Log.Information($"{model}");
                    }

                });

                Log.Information("Starting...");
                await client.Start();
                Log.Information("Started.");

                DoSubscribe(client);

                ExitEvent.WaitOne();
            }

            Log.Debug("====================================");
            Log.Debug("              STOPPING              ");
            Log.Debug("====================================");
            Log.CloseAndFlush();
        }

        private static void DoSubscribe(IWebsocketClient client)
        {
            //client.Send(new SubscribeCommand("FUT.QCL.C").ToJson());
            //client.Send(new SubscribeCommand("FUT.EB.C").ToJson());
            //client.Send(new SubscribeCommand("FUT.MCU3").ToJson());
            client.Send(new SubscribeCommand("SPO.USDCNH").ToJson());
            client.Send(new SubscribeCommand("SPO.XAUUSD").ToJson());
        }

        private static SerilogLoggerFactory InitLogging()
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            var logPath = Path.Combine(executingDir ?? string.Empty, "logs", "verbose.log");
            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
                .WriteTo.Console(LogEventLevel.Verbose,
                    theme: AnsiConsoleTheme.Literate,
                    outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {Message} {NewLine}{Exception}")
                .CreateLogger();
            Log.Logger = logger;
            return new SerilogLoggerFactory(logger);
        }

        private static void CurrentDomainOnProcessExit(object sender, EventArgs eventArgs)
        {
            Log.Warning("Exiting process");
            ExitEvent.Set();
        }

        private static void DefaultOnUnloading(AssemblyLoadContext assemblyLoadContext)
        {
            Log.Warning("Unloading process");
            ExitEvent.Set();
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Log.Warning("Canceling process");
            e.Cancel = true;
            ExitEvent.Set();
        }
    }
}
