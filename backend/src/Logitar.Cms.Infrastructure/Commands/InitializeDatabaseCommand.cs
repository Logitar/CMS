using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Commands;

public record InitializeDatabaseCommand : INotification;

public class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly EventContext _eventContext;
  private readonly CmsContext _cmsContext;

  public InitializeDatabaseCommandHandler(EventContext eventContext, CmsContext cmsContext)
  {
    _eventContext = eventContext;
    _cmsContext = cmsContext;
  }

  public async Task Handle(InitializeDatabaseCommand command, CancellationToken cancellationToken)
  {
    await _eventContext.Database.MigrateAsync(cancellationToken);
    await _cmsContext.Database.MigrateAsync(cancellationToken);
  }
}
