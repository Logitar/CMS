using Logitar.Cms.Core.Security;
using MediatR;

namespace Logitar.Cms.Core.Sessions.Events;

public record SessionCreated : SessionSaved, INotification
{
  public Pbkdf2? Secret { get; init; }
}
