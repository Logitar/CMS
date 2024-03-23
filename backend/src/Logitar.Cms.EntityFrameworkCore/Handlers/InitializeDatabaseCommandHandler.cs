using Logitar.Cms.Infrastructure.Commands;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly bool _enableMigrations;
  private readonly EventContext _eventContext;
  private readonly IdentityContext _identityContext;
  private readonly CmsContext _cmsContext;

  public InitializeDatabaseCommandHandler(IConfiguration configuration, EventContext eventContext, IdentityContext identityContext, CmsContext cmsContext)
  {
    _enableMigrations = configuration.GetValue<bool>("EnableMigrations");
    _eventContext = eventContext;
    _identityContext = identityContext;
    _cmsContext = cmsContext;
  }

  public async Task Handle(InitializeDatabaseCommand _, CancellationToken cancellationToken)
  {
    if (_enableMigrations)
    {
      await _eventContext.Database.MigrateAsync(cancellationToken);
      await _identityContext.Database.MigrateAsync(cancellationToken);
      await _cmsContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
