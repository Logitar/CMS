using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Contents;

internal class ContentCreatedEventHandler : INotificationHandler<ContentCreatedEvent>
{
  private readonly CmsContext _context;

  public ContentCreatedEventHandler(CmsContext context)
  {
    _context = context;
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
    }
  }
}
