using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class ContentItems
{
  public class ContentCreatedEventHandler : INotificationHandler<ContentCreatedEvent>
  {
    private readonly CmsContext _context;

    public ContentCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ContentCreatedEvent @event, CancellationToken cancellationToken)
    {
      ContentItemEntity? contentItem = await _context.ContentItems.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (contentItem == null)
      {
        ContentTypeEntity contentType = await _context.ContentTypes
          .SingleOrDefaultAsync(x => x.AggregateId == @event.ContentTypeId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The content type entity 'AggregateId={@event.ContentTypeId.AggregateId}' could not be found.");

        contentItem = new(contentType, @event);

        _context.ContentItems.Add(contentItem);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ContentLocaleChangedEventHandler : INotificationHandler<ContentLocaleChangedEvent>
  {
    private readonly CmsContext _context;

    public ContentLocaleChangedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ContentLocaleChangedEvent @event, CancellationToken cancellationToken)
    {
      ContentItemEntity contentItem = await _context.ContentItems
        .Include(x => x.ContentLocales)
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content item entity 'AggregateId={@event.AggregateId}' could not be found.");

      LanguageEntity? language = null;
      if (@event.LanguageId != null)
      {
        language = await _context.Languages
          .SingleOrDefaultAsync(x => x.AggregateId == @event.LanguageId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.LanguageId.Value}' could not be found.");
      }

      contentItem.SetLocale(language, @event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
