using Logitar.Cms.Core.Configurations.Events;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Configurations;

public class ConfigurationAggregate : AggregateRoot
{
  public new ConfigurationId Id { get; } = new();

  private JwtSecretUnit? _secret = null;
  public JwtSecretUnit Secret
  {
    get => _secret ?? throw new InvalidOperationException($"The {nameof(Secret)} has not been initialized yet.");
  }

  private ReadOnlyUniqueNameSettings? _uniqueNameSettings = null;
  public ReadOnlyUniqueNameSettings UniqueNameSettings
  {
    get => _uniqueNameSettings ?? throw new InvalidOperationException($"The {nameof(UniqueNameSettings)} have not been initialized yet.");
  }
  private ReadOnlyPasswordSettings? _passwordSettings = null;
  public ReadOnlyPasswordSettings PasswordSettings
  {
    get => _passwordSettings ?? throw new InvalidOperationException($"The {nameof(PasswordSettings)} have not been initialized yet.");
  }
  private bool _requireUniqueEmail = false;
  public bool RequireUniqueEmail
  {
    get => _requireUniqueEmail;
  }

  private ReadOnlyLoggingSettings? _loggingSettings = null;
  public ReadOnlyLoggingSettings LoggingSettings
  {
    get => _loggingSettings ?? throw new InvalidOperationException($"The {nameof(LoggingSettings)} have not been initialized yet.");
  }

  public ConfigurationAggregate(AggregateId id) : base(id)
  {
  }

  public static ConfigurationAggregate Initialize(ActorId actorId = default)
  {
    ConfigurationId id = new();
    ConfigurationAggregate configuration = new(id.AggregateId);

    JwtSecretUnit secret = JwtSecretUnit.Generate();
    ReadOnlyUniqueNameSettings uniqueNameSettings = new();
    ReadOnlyPasswordSettings passwordSettings = new();
    bool requireUniqueEmail = true;
    ReadOnlyLoggingSettings loggingSettings = new();
    configuration.Raise(new ConfigurationInitializedEvent(secret, uniqueNameSettings, passwordSettings, requireUniqueEmail, loggingSettings, actorId));

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
