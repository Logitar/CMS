using Logitar.Cms.Core.Security;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Events;

public record SessionRefreshed(Pbkdf2 Secret) : SessionSaved, INotification;
