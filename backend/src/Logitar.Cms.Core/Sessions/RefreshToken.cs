using Logitar.Identity.Domain.Sessions;

namespace Logitar.Cms.Core.Sessions;

internal record RefreshToken
{
  public const int SecretLength = 256 / 8;
  private const string Prefix = "RT";
  private const char Separator = '.';

  public SessionId Id { get; }
  public string Secret { get; }

  private RefreshToken(SessionId id, string secret)
  {
    Id = id;
    Secret = secret;
  }

  public static RefreshToken Decode(string refreshToken)
  {
    string[] values = refreshToken.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{refreshToken}' is not a valid refresh token.", nameof(refreshToken));
    }

    SessionId id = new(values[1]);
    string secret = values[2].FromUriSafeBase64();
    return new RefreshToken(id, secret);
  }

  public static string Encode(SessionAggregate session, string secret)
  {
    if (Convert.FromBase64String(secret).Length != SecretLength)
    {
      throw new ArgumentException($"The secret must contain exactly {SecretLength} bytes.", nameof(secret));
    }

    RefreshToken refreshToken = new(session.Id, secret);
    return refreshToken.Encode();
  }

  public string Encode() => string.Join(Separator, Prefix, Id.Value, Secret.ToUriSafeBase64());
}
