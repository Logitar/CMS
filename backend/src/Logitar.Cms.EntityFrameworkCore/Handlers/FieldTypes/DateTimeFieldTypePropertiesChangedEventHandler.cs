using Logitar.Cms.Core.Fields.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.FieldTypes;

internal class DateTimeFieldTypePropertiesChangedEventHandler : INotificationHandler<DateTimeFieldTypePropertiesChangedEvent>
{
  private readonly CmsContext _context;

  public DateTimeFieldTypePropertiesChangedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(DateTimeFieldTypePropertiesChangedEvent @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The field type entity (AggregateId={@event.AggregateId}) could not be found.");

    fieldType.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
