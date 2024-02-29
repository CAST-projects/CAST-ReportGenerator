namespace CastReporting.HL.Domain;

public class AuthToken
{
    public int ExpiresInMin { get; protected set; }
    public string Token { get; protected set; } = string.Empty;
}
