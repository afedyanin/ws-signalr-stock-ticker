namespace PrimeConnector
{
    public interface IMarketDataService
    {
        public Task ExecuteAsync(
            CancellationToken cancellationToken);
    }
}
