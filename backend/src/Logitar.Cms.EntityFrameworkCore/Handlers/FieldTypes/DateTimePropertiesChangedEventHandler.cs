using Logitar.Cms.Core.FieldTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.FieldTypes;

internal class DateTimePropertiesChangedEventHandler : INotificationHandler<DateTimePropertiesChangedEvent>
{
  private readonly CmsContext _context;

  public DateTimePropertiesChangedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(DateTimePropertiesChangedEvent @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The field type entity 'AggregateId={@event.AggregateId}' could not be found.");

    fieldType.SetProperties(@event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
