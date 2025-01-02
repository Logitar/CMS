using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class ContentTypeEvents : INotificationHandler<ContentTypeCreated>,
  INotificationHandler<ContentTypeUniqueNameChanged>,
  INotificationHandler<ContentTypeUpdated>
{
  private readonly CmsContext _context;

  public ContentTypeEvents(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(ContentTypeCreated @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType == null)
    {
      contentType = new(@event);

      _context.ContentTypes.Add(contentType);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(ContentTypeUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType != null && contentType.Version == (@event.Version - 1))
    {
      contentType.SetUniqueName(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(ContentTypeUpdated @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType != null && contentType.Version == (@event.Version - 1))
    {
      contentType.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
