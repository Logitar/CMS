using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Commands;

public record InitializeDatabaseCommand : INotification;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly EventContext _eventContext;
  private readonly IdentityContext _identityContext;
  private readonly CmsContext _cmsContext;

  public InitializeDatabaseCommandHandler(EventContext eventContext, IdentityContext identityContext, CmsContext cmsContext)
  {
    _eventContext = eventContext;
    _identityContext = identityContext;
    _cmsContext = cmsContext;
  }

  public async Task Handle(InitializeDatabaseCommand command, CancellationToken cancellationToken)
  {
    await _eventContext.Database.MigrateAsync(cancellationToken);
    await _identityContext.Database.MigrateAsync(cancellationToken);
    await _cmsContext.Database.MigrateAsync(cancellationToken);
  }
}
