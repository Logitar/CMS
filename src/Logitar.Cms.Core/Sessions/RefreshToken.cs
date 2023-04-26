using Logitar.EventSourcing;
using System.Diagnostics.CodeAnalysis;

namespace Logitar.Cms.Core.Sessions;

internal readonly struct RefreshToken
{
  private const int GuidByteCount = 16;

  private readonly Guid _id;
  private readonly byte[]? _secret;

  public RefreshToken(Guid id, byte[] secret)
  {
    if (id == Guid.Empty)
    {
      throw new ArgumentException("The identifier is required.", nameof(id));
    }
    if (secret.Length == 0)
    {
      throw new ArgumentException("The key is required.", nameof(secret));
    }

    _id = id;
    _secret = secret;
  }

  public Guid Id => _id;
  public byte[] Secret => _secret ?? Array.Empty<byte>();

  public static RefreshToken Parse(string s)
  {
    byte[] bytes = Convert.FromBase64String(s.FromUriSafeBase64());

    return new RefreshToken(new Guid(bytes.Take(GuidByteCount).ToArray()), bytes.Skip(GuidByteCount).ToArray());
  }
  public static bool TryParse(string s, out RefreshToken refreshToken)
  {
    try
    {
      refreshToken = Parse(s);
      return true;
    }
    catch (Exception)
    {
      refreshToken = default;
      return false;
    }
  }

  public static bool operator ==(RefreshToken left, RefreshToken right) => left.Equals(right);
  public static bool operator !=(RefreshToken left, RefreshToken right) => !left.Equals(right);

  public override bool Equals([NotNullWhen(true)] object? obj) => obj is RefreshToken refreshToken
    && refreshToken.Id == Id
    && refreshToken.Secret.SequenceEqual(Secret);
  public override int GetHashCode() => HashCode.Combine(Id, Secret);
  public override string ToString() => Convert.ToBase64String(Id.ToByteArray().Concat(Secret).ToArray())
    .ToUriSafeBase64();
}
