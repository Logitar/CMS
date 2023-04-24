using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Commands;

internal class MigrateDatabaseHandler : IRequestHandler<MigrateDatabase>
{
  private readonly CmsContext _cmsContext;
  private readonly EventContext _eventContext;

  public MigrateDatabaseHandler(CmsContext cmsContext, EventContext eventContext)
  {
    _cmsContext = cmsContext;
    _eventContext = eventContext;
  }

  public async Task Handle(MigrateDatabase request, CancellationToken cancellationToken)
  {
    await _cmsContext.Database.MigrateAsync(cancellationToken);
    await _eventContext.Database.MigrateAsync(cancellationToken);
  }
}
