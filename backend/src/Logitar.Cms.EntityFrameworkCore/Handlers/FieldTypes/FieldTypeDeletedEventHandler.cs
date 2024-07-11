using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.FieldTypes;

internal class FieldTypeDeletedEventHandler : INotificationHandler<FieldTypeDeletedEvent>
{
  private readonly CmsContext _context;

  public FieldTypeDeletedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(FieldTypeDeletedEvent @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (fieldType != null)
    {
      _context.FieldTypes.Remove(fieldType);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
