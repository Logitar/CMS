using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Cms.EntityFrameworkCore.Indexing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Contents;

internal class ContentCreatedEventHandler : INotificationHandler<ContentCreatedEvent>
{
  private readonly CmsContext _context;
  private readonly IPublisher _publisher;

  public ContentCreatedEventHandler(CmsContext context, IPublisher publisher)
  {
    _context = context;
    _publisher = publisher;
  }

  public async Task Handle(ContentCreatedEvent @event, CancellationToken cancellationToken)
  {
    ContentItemEntity? content = await _context.ContentItems.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (content == null)
    {
      ContentTypeEntity contentType = await _context.ContentTypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.ContentTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'AggregateId={@event.ContentTypeId.AggregateId}' could not be found.");

      content = new(contentType, @event);

      _context.ContentItems.Add(content);

      await _context.SaveChangesAsync(cancellationToken);

      ContentLocaleEntity locale = await _context.ContentLocales
        .Include(x => x.Item).ThenInclude(x => x!.ContentType).ThenInclude(x => x!.FieldDefinitions).ThenInclude(x => x.FieldType)
        .Include(x => x.Language)
        .SingleOrDefaultAsync(x => x.ContentItemId == content.ContentItemId && x.LanguageId == null, cancellationToken)
        ?? throw new InvalidOperationException($"The content locale entity 'ContentItemId={content.ContentItemId}, LanguageId=<null>' could not be found.");
      await _publisher.Publish(new UpdateFieldIndicesCommand(locale, @event.Invariant.FieldValues), cancellationToken);
    }
  }
}
