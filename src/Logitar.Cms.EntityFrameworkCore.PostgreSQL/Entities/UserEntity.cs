using Logitar.Cms.Core.Users.Events;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;

internal class UserEntity : AggregateEntity
{
  public UserEntity(UserCreated e) : base(e)
  {
    Username = e.Username;

    FirstName = e.FirstName;
    LastName = e.LastName;
    FullName = e.FullName;

    Locale = e.Locale?.Name;

    Picture = e.Picture?.ToString();
  }

  private UserEntity() : base()
  {
  }

  public int UserId { get; private set; }

  public string Username { get; private set; } = string.Empty;
  public string UsernameNormalized
  {
    get => Username.ToUpper();
    private set { }
  }

  public string? PasswordChangedById { get; private set; }
  public DateTime? PasswordChangedOn { get; private set; }
  public string? Password { get; private set; }
  public bool HasPassword
  {
    get => Password != null;
    private set { }
  }

  public string? DisabledById { get; private set; }
  public DateTime? DisabledOn { get; private set; }
  public bool IsDisabled { get; private set; }

  public DateTime? SignedInOn { get; private set; }

  public string? EmailAddress { get; private set; }
  public string? EmailAddressNormalized
  {
    get => EmailAddress?.ToUpper();
    private set { }
  }
  public string? EmailVerifiedById { get; private set; }
  public DateTime? EmailVerifiedOn { get; private set; }
  public bool IsEmailVerified { get; private set; }

  public string? FirstName { get; private set; }
  public string? LastName { get; private set; }
  public string? FullName { get; private set; }

  public string? Locale { get; private set; }

  public string? Picture { get; private set; }

  public List<SessionEntity> Sessions { get; private set; } = new();

  public void ChangePassword(PasswordChanged e)
  {
    SetVersion(e);

    PasswordChangedById = e.ActorId.Value;
    PasswordChangedOn = e.OccurredOn;
    Password = e.Password.ToString();
  }

  public void SetEmail(EmailChanged e)
  {
    Update(e);

    EmailAddress = e.Email?.Address;

    switch (e.VerificationAction)
    {
      case VerificationAction.Unverify:
        EmailVerifiedById = null;
        EmailVerifiedOn = null;
        IsEmailVerified = false;
        break;
      case VerificationAction.Verify:
        EmailVerifiedById = e.ActorId.Value;
        EmailVerifiedOn = e.OccurredOn;
        IsEmailVerified = true;
        break;
    }
  }

  public void SignIn(UserSignedIn e)
  {
    SetVersion(e);

    SignedInOn = e.OccurredOn;
  }
}
