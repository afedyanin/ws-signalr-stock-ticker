using PrimeConnector.Model;

namespace PrimeConnector
{
    public interface IAuthService
    {
        public Task<AuthToken?> Authenticate();
    }
}
