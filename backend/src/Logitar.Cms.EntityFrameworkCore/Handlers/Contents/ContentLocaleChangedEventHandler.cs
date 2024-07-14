using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Contents;

internal class ContentLocaleChangedEventHandler : INotificationHandler<ContentLocaleChangedEvent>
{
  private readonly CmsContext _context;

  public ContentLocaleChangedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(ContentLocaleChangedEvent @event, CancellationToken cancellationToken)
  {
    ContentItemEntity contentItem = await _context.ContentItems
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The content item entity 'AggregateId={@event.AggregateId}' could not be found.");

    LanguageEntity? language = null;
    if (@event.LanguageId.HasValue)
    {
      language = await _context.Languages
        .SingleOrDefaultAsync(x => x.AggregateId == @event.LanguageId.Value.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.AggregateId}' could not be found.");
    }

    contentItem.SetLocale(language, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
