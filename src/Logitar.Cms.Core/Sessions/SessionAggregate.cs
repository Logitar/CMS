using FluentValidation;
using Logitar.Cms.Core.Security;
using Logitar.Cms.Core.Sessions.Events;
using Logitar.Cms.Core.Sessions.Validators;
using Logitar.Cms.Core.Users;
using Logitar.EventSourcing;
using System.Security.Cryptography;

namespace Logitar.Cms.Core.Sessions;

public class SessionAggregate : AggregateRoot
{
  private const int SecretLength = 256 / 8;

  private Pbkdf2? _secret = null;

  public SessionAggregate(AggregateId id) : base(id) { }

  public SessionAggregate(UserAggregate user, bool isPersistent = false, DateTime? signedInOn = null,
    string? ipAddress = null, string? additionalInformation = null)
  {
    byte[]? bytes = null;
    Pbkdf2? secret = null;
    if (isPersistent)
    {
      bytes = RandomNumberGenerator.GetBytes(SecretLength);
      secret = new Pbkdf2(Convert.ToBase64String(bytes));
    }

    SessionCreated e = new()
    {
      ActorId = user.Id,
      OccurredOn = signedInOn ?? DateTime.UtcNow,
      Secret = secret,
      IpAddress = ipAddress?.CleanTrim(),
      AdditionalInformation = additionalInformation?.CleanTrim()
    };
    new SessionCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);

    if (bytes != null)
    {
      RefreshToken = new(Id.ToGuid(), bytes);
    }
  }
  protected virtual void Apply(SessionCreated e)
  {
    _secret = e.Secret;

    UserId = e.ActorId;

    IsActive = true;

    Apply((SessionSaved)e);
  }

  public AggregateId UserId { get; private set; }

  public bool IsPersistent => _secret != null;

  public bool IsActive { get; private set; }

  public string? IpAddress { get; private set; }
  public string? AdditionalInformation { get; private set; }

  internal RefreshToken? RefreshToken { get; private set; }

  public void Refresh(byte[] secretBytes, string? ipAddress = null, string? additionalInformation = null)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }
    if (_secret?.IsMatch(Convert.ToBase64String(secretBytes)) != true)
    {
      throw new InvalidCredentialsException($"The secret '{Convert.ToBase64String(secretBytes)}' did not match the session '{this}'.");
    }

    secretBytes = RandomNumberGenerator.GetBytes(SecretLength);
    Pbkdf2 secret = new(Convert.ToBase64String(secretBytes));

    SessionRefreshed e = new(secret)
    {
      ActorId = UserId,
      IpAddress = ipAddress?.CleanTrim(),
      AdditionalInformation = additionalInformation?.CleanTrim()
    };
    new SessionRefreshedValidator().ValidateAndThrow(e);

    ApplyChange(e);

    RefreshToken = new(Id.ToGuid(), secretBytes);
  }
  protected virtual void Apply(SessionRefreshed e)
  {
    _secret = e.Secret;

    Apply((SessionSaved)e);
  }

  public void SignOut(AggregateId actorId)
  {
    if (!IsActive)
    {
      throw new SessionIsNotActiveException(this);
    }

    ApplyChange(new SessionSignedOut { ActorId = actorId });
  }
  protected virtual void Apply(SessionSignedOut _) => IsActive = false;

  private void Apply(SessionSaved e)
  {
    IpAddress = e.IpAddress;
    AdditionalInformation = e.AdditionalInformation;
  }
}
