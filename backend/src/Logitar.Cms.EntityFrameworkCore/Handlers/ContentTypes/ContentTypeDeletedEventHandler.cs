using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.ContentTypes;

internal class ContentTypeDeletedEventHandler : INotificationHandler<ContentTypeDeletedEvent>
{
  private readonly CmsContext _context;

  public ContentTypeDeletedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(ContentTypeDeletedEvent @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (contentType != null)
    {
      _context.ContentTypes.Remove(contentType);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
