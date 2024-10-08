using Logitar.Cms.Contracts.Configurations;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;
using MediatR;

namespace Logitar.Cms.Core.Configurations;

public class Configuration : AggregateRoot
{
  public new ConfigurationId Id { get; } = new();

  private JwtSecret? _secret = null;
  public JwtSecret Secret => _secret ?? throw new InvalidOperationException($"The '{nameof(Secret)}' has not been initialized yet.");

  private UniqueNameSettings? _uniqueNameSettings = null;
  public UniqueNameSettings UniqueNameSettings => _uniqueNameSettings ?? throw new InvalidOperationException($"The '{nameof(UniqueNameSettings)}' has not been initialized yet.");
  private PasswordSettings? _passwordSettings = null;
  public PasswordSettings PasswordSettings => _passwordSettings ?? throw new InvalidOperationException($"The '{nameof(PasswordSettings)}' has not been initialized yet.");
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail => _requireUniqueEmail;
  public IUserSettings UserSettings => new UserSettings
  {
    UniqueName = UniqueNameSettings,
    Password = PasswordSettings,
    RequireUniqueEmail = RequireUniqueEmail
  };

  private LoggingSettings? _loggingSettings = null;
  public LoggingSettings LoggingSettings => _loggingSettings ?? throw new InvalidOperationException($"The {nameof(LoggingSettings)} has not been initialized yet.");

  public Configuration() : base(new ConfigurationId().AggregateId)
  {
  }

  public static Configuration Initialize(ActorId actorId)
  {
    Configuration configuration = new();

    JwtSecret secret = JwtSecret.Generate();
    UniqueNameSettings uniqueNameSettings = new("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+");
    PasswordSettings passwordSettings = new(
      requiredLength: 8,
      requiredUniqueChars: 8,
      requireNonAlphanumeric: true,
      requireLowercase: true,
      requireUppercase: true,
      requireDigit: true,
      hashingStrategy: "PBKDF2");
    bool requireUniqueEmail = true;
    LoggingSettings loggingSettings = new(LoggingExtent.ActivityOnly, onlyErrors: false);
    configuration.Raise(new InitializedEvent(secret, uniqueNameSettings, passwordSettings, requireUniqueEmail, loggingSettings), actorId);

    return configuration;
  }
  protected virtual void Apply(InitializedEvent @event)
  {
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;

    _loggingSettings = @event.LoggingSettings;
  }

  public class InitializedEvent : DomainEvent, INotification
  {
    public JwtSecret Secret { get; }

    public UniqueNameSettings UniqueNameSettings { get; }
    public PasswordSettings PasswordSettings { get; }
    public bool RequireUniqueEmail { get; }

    public LoggingSettings LoggingSettings { get; }

    public InitializedEvent(
      JwtSecret secret,
      UniqueNameSettings uniqueNameSettings,
      PasswordSettings passwordSettings,
      bool requireUniqueEmail,
      LoggingSettings loggingSettings)
    {
      Secret = secret;

      UniqueNameSettings = uniqueNameSettings;
      PasswordSettings = passwordSettings;
      RequireUniqueEmail = requireUniqueEmail;

      LoggingSettings = loggingSettings;
    }
  }
}
