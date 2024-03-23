using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class ContentTypes
{
  public class ContentTypeCreatedEventHandler : INotificationHandler<ContentTypeCreatedEvent>
  {
    private readonly CmsContext _context;

    public ContentTypeCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ContentTypeCreatedEvent @event, CancellationToken cancellationToken)
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

  public class ContentTypeUpdatedEventHandler : INotificationHandler<ContentTypeUpdatedEvent>
  {
    private readonly CmsContext _context;

    public ContentTypeUpdatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(ContentTypeUpdatedEvent @event, CancellationToken cancellationToken)
    {
      ContentTypeEntity contentType = await _context.ContentTypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The content type entity 'AggregateId={@event.AggregateId}' could not be found.");

      contentType.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
