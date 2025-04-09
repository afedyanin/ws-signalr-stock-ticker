
using Microsoft.Extensions.Options;
using PrimeConnector.Model;

namespace PrimeConnector.Services
{
    public class ConfigSymbolProvider : ISymbolProvider
    {
        private readonly PrimeSettings _primeSettings;

        public ConfigSymbolProvider(IOptions<PrimeSettings> options)
        {
            _primeSettings = options.Value;
        }
        public IEnumerable<string> GetSymbols() => _primeSettings.Symbols;
    }
}
