using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Cms.EntityFrameworkCore.Indexing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Contents;

internal class ContentLocaleChangedEventHandler : INotificationHandler<ContentLocaleChangedEvent>
{
  private readonly CmsContext _context;
  private readonly IPublisher _publisher;

  public ContentLocaleChangedEventHandler(CmsContext context, IPublisher publisher)
  {
    _context = context;
    _publisher = publisher;
  }

  public async Task Handle(ContentLocaleChangedEvent @event, CancellationToken cancellationToken)
  {
    ContentItemEntity contentItem = await _context.ContentItems
      .Include(x => x.Locales)
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

    ContentLocaleEntity locale = await _context.ContentLocales
      .Include(x => x.Item).ThenInclude(x => x!.ContentType).ThenInclude(x => x!.FieldDefinitions).ThenInclude(x => x.FieldType)
      .Include(x => x.Language)
      .SingleOrDefaultAsync(x => x.ContentItemId == contentItem.ContentItemId && (language == null ? x.LanguageId == null : x.LanguageId == language.LanguageId), cancellationToken)
      ?? throw new InvalidOperationException($"The content locale entity 'ContentItemId={contentItem.ContentItemId}, LanguageId={language?.LanguageId.ToString() ?? "<null>"}' could not be found.");
    await _publisher.Publish(new UpdateFieldIndicesCommand(locale, @event.Locale.FieldValues), cancellationToken);
  }
}
