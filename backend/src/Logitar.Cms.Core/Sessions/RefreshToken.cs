namespace Logitar.Cms.Core.Sessions;

internal record RefreshToken
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "RT";
  private const char Separator = '.';

  public Guid SessionId { get; }
  public string Secret { get; }

  public RefreshToken(Guid sessionId, string secret)
  {
    if (Convert.FromBase64String(secret).Length != SecretLength)
    {
      throw new ArgumentOutOfRangeException(nameof(secret));
    }

    SessionId = sessionId;
    Secret = secret;
  }

  public static RefreshToken Decode(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    Guid sessionId = new Guid(Convert.FromBase64String(values[1].FromUriSafeBase64()));
    string secret = values[2].FromUriSafeBase64();
    return new RefreshToken(sessionId, secret);
  }

  public string Encode() => string.Join(Separator, Prefix, Convert.ToBase64String(SessionId.ToByteArray()).ToUriSafeBase64(), Secret.ToUriSafeBase64());
}
