using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class ContentEvents : INotificationHandler<ContentCreated>,
  INotificationHandler<ContentLocaleChanged>
{
  private readonly CmsContext _context;

  public ContentEvents(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(ContentCreated @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content == null)
    {
      ContentTypeEntity contentType = await _context.ContentTypes
        .SingleOrDefaultAsync(x => x.StreamId == @event.ContentTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'StreamId={@event.ContentTypeId}' could not be found.");

      content = new(contentType, @event);

      _context.Contents.Add(content);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(ContentLocaleChanged @event, CancellationToken cancellationToken)
  {
    ContentEntity? content = await _context.Contents
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (content != null && content.Version == (@event.Version - 1))
    {
      LanguageEntity? language = null;
      if (@event.LanguageId.HasValue)
      {
        language = await _context.Languages
          .SingleOrDefaultAsync(x => x.StreamId == @event.LanguageId.Value.Value, cancellationToken)
          ?? throw new InvalidOperationException($"The language entity 'StreamId={@event.LanguageId}' could not be found.");
      }

      content.SetLocale(language, @event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
