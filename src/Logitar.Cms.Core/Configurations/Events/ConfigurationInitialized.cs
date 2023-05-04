using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Configurations.Events;

public record ConfigurationInitialized(string Secret, ReadOnlyLoggingSettings LoggingSettings,
  ReadOnlyUsernameSettings UsernameSettings, ReadOnlyPasswordSettings PasswordSettings) : DomainEvent, INotification;
