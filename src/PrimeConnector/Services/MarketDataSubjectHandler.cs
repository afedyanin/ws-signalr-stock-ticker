using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using PrimeConnector.Extensions;
using PrimeConnector.Model;

namespace PrimeConnector.Services
{
    internal class MarketDataSubjectHandler : IMarketDataMessageHandler, IMarketDataObservable
    {
        private readonly Subject<MarketQuote> _marketQuotes;
        private readonly Subject<SubscribeStatus> _subscribeStatusSubject;
        private readonly ILogger<MarketDataSubjectHandler> _logger;

        private readonly Dictionary<string, MarketQuote> _quotes = [];
        private readonly Dictionary<string, SubscribeStatus> _subscribtions = [];

        public MarketDataSubjectHandler(ILogger<MarketDataSubjectHandler> logger)
        {
            _logger = logger;
            _marketQuotes = new Subject<MarketQuote>();
            _subscribeStatusSubject = new Subject<SubscribeStatus>();
        }

        public IEnumerable<MarketQuote> GetMarketQuotes() => _quotes.Values;

        public IEnumerable<SubscribeStatus> GetSubscriptions() => _subscribtions.Values;

        public IObservable<MarketQuote> StreamMerketData() => _marketQuotes;

        public IObservable<SubscribeStatus> StreamSubscribeStatus() => _subscribeStatusSubject;

        public Task Handle(StreamMessage streamMessage, CancellationToken cancellationToken = default)
        {
            var quote = streamMessage.ToQuote();

            if (quote != null)
            {
                var instrument = quote.Instrument;
                if (instrument != null)
                {
                    _quotes[instrument] = quote;
                    _marketQuotes.OnNext(quote);
                    _logger.LogDebug("Market quote received: {quote}", quote);
                }
            }

            return Task.CompletedTask;
        }

        public Task Handle(SubscribeStatus subscribeStatus, CancellationToken cancellationToken = default)
        {
            var instrument = subscribeStatus.Body?.Instrument;
            
            if (instrument != null)
            {
                _subscribtions[instrument] = subscribeStatus;
                _subscribeStatusSubject.OnNext(subscribeStatus);
                _logger.LogDebug("SubscribeStatus: {message}", subscribeStatus);
            }

            return Task.CompletedTask;
        }
    }
}
