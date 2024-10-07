using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Contents;

internal class ContentDeletedEventHandler : INotificationHandler<ContentDeletedEvent>
{
  private readonly CmsContext _context;

  public ContentDeletedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(ContentDeletedEvent @event, CancellationToken cancellationToken)
  {
    ContentItemEntity? content = await _context.ContentItems
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (content != null)
    {
      _context.ContentItems.Remove(content);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
