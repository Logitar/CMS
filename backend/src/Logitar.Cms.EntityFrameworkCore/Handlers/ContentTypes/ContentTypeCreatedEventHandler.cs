using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.ContentTypes;

internal class ContentTypeCreatedEventHandler : INotificationHandler<ContentTypeCreatedEvent>
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
