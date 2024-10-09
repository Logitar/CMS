using Logitar.Cms.Core.Contents;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class ContentEvents
{
  public class ContentCreatedEventHandler : INotificationHandler<Content.CreatedEvent>
  {
    private readonly CmsContext _context;

    public ContentCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(Content.CreatedEvent @event, CancellationToken cancellationToken)
    {
      ContentEntity? content = await _context.Contents.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (content == null)
      {
        ContentTypeEntity contentType = await _context.ContentTypes
          .SingleOrDefaultAsync(x => x.AggregateId == @event.ContentTypeId.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The content type entity 'AggregateId={@event.ContentTypeId}' could not be found.");

        content = new(contentType, @event);

        _context.Contents.Add(content);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ContentLocaleChangedEventHandler : INotificationHandler<Content.LocaleChangedEvent>
  {
    private readonly CmsContext _context;

    public ContentLocaleChangedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(Content.LocaleChangedEvent @event, CancellationToken cancellationToken)
    {
      ContentEntity content = await _context.Contents
        .Include(x => x.Locales)
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content entity 'AggregateId={@event.AggregateId}' could not be found.");

      LanguageEntity? language = null;
      if (@event.LanguageId.HasValue)
      {
        language = await _context.Languages
          .SingleOrDefaultAsync(x => x.AggregateId == @event.LanguageId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The language entity 'AggregateId={@event.LanguageId}' could not be found.");
      }

      content.SetLocale(language, @event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
