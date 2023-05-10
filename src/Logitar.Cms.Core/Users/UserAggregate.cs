using FluentValidation;
using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Security;
using Logitar.Cms.Core.Sessions;
using Logitar.Cms.Core.Users.Events;
using Logitar.Cms.Core.Users.Validators;
using Logitar.EventSourcing;
using System.Globalization;

namespace Logitar.Cms.Core.Users;

public class UserAggregate : AggregateRoot
{
  private Pbkdf2? _password = null;

  public UserAggregate(AggregateId id) : base(id) { }

  public UserAggregate(AggregateId actorId, ConfigurationAggregate configuration, string username,
    string? firstName = null, string? lastName = null, CultureInfo? locale = null, Uri? picture = null,
    AggregateId? id = null) : base(id ?? AggregateId.NewId())
  {
    UserCreated e = new(username.Trim())
    {
      ActorId = actorId,
      FirstName = firstName?.CleanTrim(),
      LastName = lastName?.CleanTrim(),
      FullName = GetFullName(firstName, lastName),
      Locale = locale,
      Picture = picture
    };
    new UserCreatedValidator(configuration.UsernameSettings).ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(UserCreated e)
  {
    Username = e.Username;

    Apply((UserSaved)e);
  }

  public string Username { get; private set; } = string.Empty;
  public bool HasPassword => _password != null;

  public ReadOnlyEmail? Email { get; private set; }

  public string? FirstName { get; private set; }
  public string? LastName { get; private set; }
  public string? FullName { get; private set; }

  public CultureInfo? Locale { get; private set; }

  public Uri? Picture { get; private set; }

  public void ChangePassword(ConfigurationAggregate configuration, string password, string? current = null, AggregateId? actorId = null)
  {
    if (current != null && _password?.IsMatch(current) != true)
    {
      throw new InvalidCredentialsException("The specified password did not match.");
    }

    new PasswordValidator(configuration.PasswordSettings).ValidateAndThrow(password);

    ApplyChange(new PasswordChanged(new Pbkdf2(password))
    {
      ActorId = actorId ?? Id
    });
  }
  protected virtual void Apply(PasswordChanged e) => _password = e.Password;

  public void SetEmail(ReadOnlyEmail? email, AggregateId? actorId = null)
  {
    bool isModified = email?.Address != Email?.Address;

    EmailChanged e = new()
    {
      ActorId = actorId ?? Id,
      Email = email,
      VerificationAction = email?.IsVerified == true ? VerificationAction.Verify
        : (isModified ? VerificationAction.Unverify : null)
    };
    new EmailChangedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(EmailChanged e) => Email = e.Email;

  public SessionAggregate SignIn(string password, bool remember = false, string? ipAddress = null,
    string? additionalInformation = null)
  {
    if (_password?.IsMatch(password) != true)
    {
      throw new InvalidCredentialsException($"The password '{password}' did not match the user '{this}'.");
    }

    UserSignedIn e = new() { ActorId = Id };
    ApplyChange(e);

    return new SessionAggregate(this, remember, e.OccurredOn, ipAddress, additionalInformation);
  }
  protected virtual void Apply(UserSignedIn _) { }

  public void Update(AggregateId actorId, string? firstName, string? lastName, CultureInfo? locale, Uri? picture)
  {
    UserUpdated e = new()
    {
      ActorId = actorId,
      FirstName = firstName?.CleanTrim(),
      LastName = lastName?.CleanTrim(),
      FullName = GetFullName(firstName, lastName),
      Locale = locale,
      Picture = picture
    };
    new UserUpdatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(UserUpdated e) => Apply((UserSaved)e);

  private void Apply(UserSaved e)
  {
    FirstName = e.FirstName;
    LastName = e.LastName;
    FullName = e.FullName;

    Locale = e.Locale;

    Picture = e.Picture;
  }

  public override string ToString() => FullName == null
    ? $"{Username} | {base.ToString()}"
    : $"{FullName} ({Username}) | {base.ToString()}";

  internal static string? GetFullName(params string?[] names) => string.Join(' ', names
    .SelectMany(name => name?.Split() ?? Array.Empty<string>())
    .Where(name => !string.IsNullOrEmpty(name)))?.CleanTrim();
}
