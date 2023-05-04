using Logitar.Cms.Core.Languages.Events;
using Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Handlers.Languages;

internal class SetDefaultLanguageHandler : INotificationHandler<SetDefaultLanguage>
{
  private readonly CmsContext _context;

  public SetDefaultLanguageHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(SetDefaultLanguage notification, CancellationToken cancellationToken)
  {
    LanguageEntity language = await _context.Languages
      .SingleOrDefaultAsync(x => x.AggregateId == notification.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The language entity '{notification.AggregateId}' could not be found.");

    language.SetDefault(notification);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
