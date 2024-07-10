using Logitar.Cms.Core.Logging;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Cms.Infrastructure;

namespace Logitar.Cms.EntityFrameworkCore.Repositories;

internal class LogRepository : ILogRepository
{
  private readonly CmsContext _context;
  private readonly JsonSerializerOptions _serializerOptions = new();

  public LogRepository(CmsContext context, IServiceProvider serviceProvider)
  {
    _context = context;
    _serializerOptions.Converters.AddRange(serviceProvider.GetLogitarCmsJsonConverters());
  }

  public async Task SaveAsync(Log log, CancellationToken cancellationToken)
  {
    LogEntity entity = new(log, _serializerOptions);

    _context.Logs.Add(entity);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
