using Microsoft.Extensions.Options;
using PrimeConnector.Model;
using System.Text.Json;

namespace PrimeConnector.Services;

internal class AuthService : IAuthService
{
    private static readonly JsonSerializerOptions _serializerOptions =
        new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true,
        };

    private readonly HttpClient _httpClient;
    private readonly PrimeSettings _settings;

    public AuthService(
        HttpClient httpClient,
        IOptions<PrimeSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
    }

    public async Task<AuthToken?> Authenticate()
    {
        var kvp = new KeyValuePair<string, string>[]
        {
            new("grant_type", "client_credentials"),
            new("client_id", _settings.ClientId),
            new("client_secret", _settings.ClientSecret),
        };

        var content = new FormUrlEncodedContent(kvp);
        var response = await _httpClient.PostAsync(_settings.AuthUrl, content);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<AuthToken>(json, _serializerOptions);

        return token;
    }
}
