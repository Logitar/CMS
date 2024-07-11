using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.FieldTypes;

internal class FieldTypeCreatedEventHandler : INotificationHandler<FieldTypeCreatedEvent>
{
  private readonly CmsContext _context;

  public FieldTypeCreatedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(FieldTypeCreatedEvent @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken);
    if (fieldType == null)
    {
      fieldType = new(@event);

      _context.FieldTypes.Add(fieldType);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
