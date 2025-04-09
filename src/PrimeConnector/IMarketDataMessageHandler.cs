using PrimeConnector.Model;

namespace PrimeConnector
{
    public interface IMarketDataMessageHandler
    {
        public Task Handle(
            StreamMessage streamMessage, 
            CancellationToken cancellationToken = default);
        
        public Task Handle(
            SubscribeStatus subscribeStatus, 
            CancellationToken cancellationToken = default);
    }
}
