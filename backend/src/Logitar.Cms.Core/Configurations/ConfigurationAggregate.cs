using Logitar.Cms.Core.Configurations.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Cms.Core.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public new ConfigurationId Id { get; } = new();

  private JwtSecretUnit? _secret = null;
  public JwtSecretUnit Secret => _secret ?? throw new InvalidOperationException($"The '{nameof(Secret)}' has not been initialized yet.");

  private ReadOnlyUniqueNameSettings? _uniqueNameSettings = null;
  public ReadOnlyUniqueNameSettings UniqueNameSettings => _uniqueNameSettings ?? throw new InvalidOperationException($"The '{nameof(UniqueNameSettings)}' has not been initialized yet.");
  private ReadOnlyPasswordSettings? _passwordSettings = null;
  public ReadOnlyPasswordSettings PasswordSettings => _passwordSettings ?? throw new InvalidOperationException($"The '{nameof(PasswordSettings)}' has not been initialized yet.");
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail => _requireUniqueEmail;
  public IUserSettings UserSettings => new UserSettings
  {
    UniqueName = UniqueNameSettings,
    Password = PasswordSettings,
    RequireUniqueEmail = RequireUniqueEmail
  };

  private ReadOnlyLoggingSettings? _loggingSettings = null;
  public ReadOnlyLoggingSettings LoggingSettings => _loggingSettings ?? throw new InvalidOperationException($"The {nameof(LoggingSettings)} has not been initialized yet.");

  public static ConfigurationAggregate Initialize(ActorId actorId = default)
  {
    ConfigurationAggregate configuration = new();

    JwtSecretUnit secret = JwtSecretUnit.Generate();
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passwordSettings = new();
    bool requireUniqueEmail = true;
    ReadOnlyLoggingSettings loggingSettings = new();
    configuration.Raise(new ConfigurationInitializedEvent(secret, uniqueNameSettings, passwordSettings, requireUniqueEmail, loggingSettings), actorId);

    return configuration;
  }
  protected virtual void Apply(ConfigurationInitializedEvent @event)
  {
    _secret = @event.Secret;

    _uniqueNameSettings = @event.UniqueNameSettings;
    _passwordSettings = @event.PasswordSettings;
    _requireUniqueEmail = @event.RequireUniqueEmail;

    _loggingSettings = @event.LoggingSettings;
  }
}
