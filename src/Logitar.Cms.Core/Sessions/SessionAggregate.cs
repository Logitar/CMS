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

  public SessionAggregate(UserAggregate user, DateTime signedInOn, bool isPersistent = false,
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
      OccurredOn = signedInOn,
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

    IpAddress = e.IpAddress;
    AdditionalInformation = e.AdditionalInformation;
  }

  public AggregateId UserId { get; private set; }

  public bool IsPersistent => _secret != null;

  public bool IsActive { get; private set; }

  public string? IpAddress { get; private set; }
  public string? AdditionalInformation { get; private set; }

  internal RefreshToken? RefreshToken { get; private set; }
}
