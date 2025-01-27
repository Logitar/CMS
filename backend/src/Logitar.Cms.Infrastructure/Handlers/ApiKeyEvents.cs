using Logitar.Cms.Core.Caching;
using Logitar.EventSourcing;
using Logitar.Identity.Core.ApiKeys.Events;
using MediatR;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class ApiKeyEvents : INotificationHandler<ApiKeyUpdated>
{
  private readonly ICacheService _cacheService;

  public ApiKeyEvents(ICacheService cacheService)
  {
    _cacheService = cacheService;
  }

  public Task Handle(ApiKeyUpdated @event, CancellationToken cancellationToken)
  {
    ActorId actorId = new(@event.StreamId.Value);
    _cacheService.RemoveActor(actorId);
    return Task.CompletedTask;
  }
}
