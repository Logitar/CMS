using Logitar.Cms.Core.Caching;
using Logitar.Cms.Core.Logging;
using Logitar.EventSourcing;
using Logitar.Identity.Infrastructure;
using Logitar.Identity.Infrastructure.Handlers;
using MediatR;

namespace Logitar.Cms.Infrastructure;

internal class CmsEventBus : EventBus
{
  private readonly ICacheService _cacheService;
  private readonly ILoggingService _loggingService;

  public CmsEventBus(
    ICacheService cacheService,
    ILoggingService loggingService,
    IPublisher publisher,
    IApiKeyEventHandler apiKeyEventHandler,
    IOneTimePasswordEventHandler oneTimePasswordEventHandler,
    IRoleEventHandler roleEventHandler,
    ISessionEventHandler sessionEventHandler,
    IUserEventHandler userEventHandler
  ) : base(publisher, apiKeyEventHandler, oneTimePasswordEventHandler, roleEventHandler, sessionEventHandler, userEventHandler)
  {
    _cacheService = cacheService;
    _loggingService = loggingService;
  }

  public override async Task PublishAsync(DomainEvent @event, CancellationToken cancellationToken)
  {
    _loggingService.Report(@event);

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
