using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.ContentTypes;

internal class FieldDefinitionChangedEventHandler : INotificationHandler<FieldDefinitionChangedEvent>
{
  private readonly CmsContext _context;

  public FieldDefinitionChangedEventHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(FieldDefinitionChangedEvent @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity contentType = await _context.ContentTypes
      .Include(x => x.FieldDefinitions)
      .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The content type entity 'AggregateId={@event.AggregateId}' could not be found.");

    FieldTypeEntity fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.AggregateId == @event.FieldDefinition.FieldTypeId.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The field type entity 'AggregateId={@event.FieldDefinition.FieldTypeId.AggregateId}' could not be found.");

    contentType.SetFieldDefinition(fieldType, @event);

    await _context.SaveChangesAsync(cancellationToken);
  }
}
