using Logitar.Cms.Infrastructure.Commands;
using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers;

internal class InitializeDatabaseHandler : INotificationHandler<InitializeDatabase>
{
  private readonly CmsContext _cmsContext;
  private readonly IConfiguration _configuration;
  private readonly EventContext _eventContext;

  public InitializeDatabaseHandler(CmsContext cmsContext,
    IConfiguration configuration,
    EventContext eventContext)
  {
    _cmsContext = cmsContext;
    _configuration = configuration;
    _eventContext = eventContext;
  }

  public async Task Handle(InitializeDatabase notification, CancellationToken cancellationToken)
  {
    if (_configuration.GetValue<bool>("MigrateDatabase"))
    {
      await _cmsContext.Database.MigrateAsync(cancellationToken);
      await _eventContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
