using Logitar.Cms.Core.Languages.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Languages;

internal class LanguageCreatedHandler : INotificationHandler<LanguageCreated>
{
  private readonly CmsContext _context;

  public LanguageCreatedHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(LanguageCreated notification, CancellationToken cancellationToken)
  {
    LanguageEntity language = new(notification);

    _context.Languages.Add(language);
    await _context.SaveChangesAsync(cancellationToken);
  }
}
