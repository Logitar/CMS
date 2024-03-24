using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Events;

public record ConfigurationInitializedEvent : DomainEvent, INotification
{
  public LocaleUnit? DefaultLocale { get; }
  public JwtSecretUnit Secret { get; }

  public ReadOnlyUniqueNameSettings UniqueNameSettings { get; }
  public ReadOnlyPasswordSettings PasswordSettings { get; }
  public bool RequireUniqueEmail { get; }

  public ReadOnlyLoggingSettings LoggingSettings { get; }

  public ConfigurationInitializedEvent(LocaleUnit? defaultLocale, JwtSecretUnit secret, ReadOnlyUniqueNameSettings uniqueNameSettings,
    ReadOnlyPasswordSettings passwordSettings, bool requireUniqueEmail, ReadOnlyLoggingSettings loggingSettings, ActorId actorId)
  {
    DefaultLocale = defaultLocale;
    Secret = secret;
    UniqueNameSettings = uniqueNameSettings;
    PasswordSettings = passwordSettings;
    RequireUniqueEmail = requireUniqueEmail;
    LoggingSettings = loggingSettings;
    ActorId = actorId;
  }
}
