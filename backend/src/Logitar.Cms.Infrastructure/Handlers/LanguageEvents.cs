using Logitar.Cms.Core.Localization.Events;
using Logitar.Cms.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class LanguageEvents : INotificationHandler<LanguageCreated>,
  INotificationHandler<LanguageLocaleChanged>,
  INotificationHandler<LanguageSetDefault>
{
  private readonly CmsContext _context;

  public LanguageEvents(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(LanguageCreated @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language == null)
    {
      language = new(@event);

      _context.Languages.Add(language);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(LanguageLocaleChanged @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language != null && language.Version == (@event.Version - 1))
    {
      language.SetLocale(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(LanguageSetDefault @event, CancellationToken cancellationToken)
  {
    LanguageEntity? language = await _context.Languages
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (language != null && language.Version == (@event.Version - 1))
    {
      language.SetDefault(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
