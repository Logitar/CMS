using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Events;

public class ConfigurationInitializedEvent : DomainEvent, INotification
{
  public JwtSecretUnit Secret { get; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; }
  public ReadOnlyPasswordSettings PasswordSettings { get; }
  public bool RequireUniqueEmail { get; }

  public ReadOnlyLoggingSettings LoggingSettings { get; }

  public ConfigurationInitializedEvent(
    JwtSecretUnit secret,
    ReadOnlyUniqueNameSettings uniqueNameSettings,
    ReadOnlyPasswordSettings passwordSettings,
    bool requireUniqueEmail,
    ReadOnlyLoggingSettings loggingSettings)
  {
    Secret = secret;

    UniqueNameSettings = uniqueNameSettings;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;

    LoggingSettings = loggingSettings;
  }
}
