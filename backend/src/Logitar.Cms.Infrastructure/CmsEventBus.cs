using Logitar.Cms.Core.Caching;
using Logitar.EventSourcing;
using Logitar.Identity.Infrastructure;
using Logitar.Identity.Infrastructure.Handlers;
using MediatR;

namespace Logitar.Cms.Infrastructure;

internal class CmsEventBus : EventBus
{
  private readonly ICacheService _cacheService;

  public CmsEventBus(ICacheService cacheService, IPublisher publisher, IApiKeyEventHandler apiKeyEventHandler, IOneTimePasswordEventHandler oneTimePasswordEventHandler, IRoleEventHandler roleEventHandler, ISessionEventHandler sessionEventHandler, IUserEventHandler userEventHandler)
    : base(publisher, apiKeyEventHandler, oneTimePasswordEventHandler, roleEventHandler, sessionEventHandler, userEventHandler)
  {
    _cacheService = cacheService;
  }

  public override async Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    await base.PublishAsync(@event, cancellationToken);

    string? @namespace = @event.GetType().Namespace;
    switch (@namespace)
    {
      case "Logitar.Identity.Domain.ApiKeys.Events":
      case "Logitar.Identity.Domain.Users.Events":
        ActorId actorId = new(@event.AggregateId.Value);
        _cacheService.RemoveActor(actorId);
        break;
    }
  }
}
