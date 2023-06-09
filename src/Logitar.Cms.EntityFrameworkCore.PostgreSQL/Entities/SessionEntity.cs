﻿using Logitar.Cms.Core.Sessions.Events;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;

internal class SessionEntity : AggregateEntity
{
  public SessionEntity(SessionCreated e, UserEntity user) : base(e)
  {
    User = user;
    UserId = user.UserId;

    Secret = e.Secret?.ToString();

    IsActive = true;

    Apply(e);
  }

  private SessionEntity() : base()
  {
  }

  public int SessionId { get; private set; }

  public UserEntity? User { get; private set; }
  public int UserId { get; private set; }

  public string? Secret { get; private set; }
  public bool IsPersistent
  {
    get => Secret != null;
    private set { }
  }

  public string? SignedOutById { get; private set; }
  public DateTime? SignedOutOn { get; private set; }
  public bool IsActive { get; private set; }

  public string? IpAddress { get; private set; }
  public string? AdditionalInformation { get; private set; }

  public void Refresh(SessionRefreshed e)
  {
    Update(e);

    Secret = e.Secret.ToString();

    Apply(e);
  }

  public void SignOut(SessionSignedOut e)
  {
    SetVersion(e);

    SignedOutById = e.ActorId.Value;
    SignedOutOn = e.OccurredOn;
    IsActive = false;
  }

  private void Apply(SessionSaved e)
  {
    IpAddress = e.IpAddress;
    AdditionalInformation = e.AdditionalInformation;
  }
}
