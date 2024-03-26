using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Events;

public record ConfigurationInitializedEvent(
  JwtSecretUnit Secret,
  ReadOnlyUniqueNameSettings UniqueNameSettings,
  ReadOnlyPasswordSettings PasswordSettings,
  bool RequireUniqueEmail,
  ReadOnlyLoggingSettings LoggingSettings
) : DomainEvent, INotification;
