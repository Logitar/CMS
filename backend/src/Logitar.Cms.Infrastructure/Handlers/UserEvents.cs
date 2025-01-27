using Logitar.Cms.Core.Caching;
using Logitar.EventSourcing;
using Logitar.Identity.Core.Users.Events;
using MediatR;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class UserEvents : INotificationHandler<UserEmailChanged>,
  INotificationHandler<UserUniqueNameChanged>,
  INotificationHandler<UserUpdated>
{
  private readonly ICacheService _cacheService;

  public UserEvents(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task Handle(UserEmailChanged @event, CancellationToken cancellationToken)
  {
    RemoveActor(@event);
    return Task.CompletedTask;
  }

  public Task Handle(UserUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    RemoveActor(@event);
    return Task.CompletedTask;
  }

  public Task Handle(UserUpdated @event, CancellationToken cancellationToken)
  {
    RemoveActor(@event);
    return Task.CompletedTask;
  }

  private void RemoveActor(DomainEvent @event)
  {
    ActorId actorId = new(@event.StreamId.Value);
    _cacheService.RemoveActor(actorId);
  }
}
