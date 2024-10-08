using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class ContentTypeEvents
{
  public class ContentTypeCreatedEventHandler : INotificationHandler<ContentType.CreatedEvent>
  {
    private readonly CmsContext _context;

    public ContentTypeCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ContentType.CreatedEvent @event, CancellationToken cancellationToken)
    {
      ContentTypeEntity? contentType = await _context.ContentTypes.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
      if (contentType == null)
      {
        contentType = new(@event);

        _context.ContentTypes.Add(contentType);

        await _context.SaveChangesAsync(cancellationToken);
      }
    }
  }

  public class ContentTypeUpdatedEventHandler : INotificationHandler<ContentType.UpdatedEvent>
  {
    private readonly CmsContext _context;

    public ContentTypeUpdatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ContentType.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      ContentTypeEntity contentType = await _context.ContentTypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'AggregateId={@event.AggregateId}' could not be found.");

      contentType.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
