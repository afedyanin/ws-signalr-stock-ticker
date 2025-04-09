namespace PrimeConnector.Model;

public class AuthToken
{
    public string? AccessToken { get; set; }

    public int ExpiresIn { get; set; }

    public int RefreshExpiresIn { get; set; }

    public string? TokenType { get; set; }
}
