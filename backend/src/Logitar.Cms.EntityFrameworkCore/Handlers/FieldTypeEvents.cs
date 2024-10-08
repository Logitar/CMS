using Logitar.Cms.Core.FieldTypes;
using Logitar.Cms.EntityFrameworkCore.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers;

internal static class FieldTypeEvents
{
  public class FieldTypeCreatedEventHandler : INotificationHandler<FieldType.CreatedEvent>
  {
    private readonly CmsContext _context;

    public FieldTypeCreatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(FieldType.CreatedEvent @event, CancellationToken cancellationToken)
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

  public class FieldTypeStringPropertiesChangedEventHandler : INotificationHandler<FieldType.StringPropertiesChangedEvent>
  {
    private readonly CmsContext _context;

    public FieldTypeStringPropertiesChangedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(FieldType.StringPropertiesChangedEvent @event, CancellationToken cancellationToken)
    {
      FieldTypeEntity fieldType = await _context.FieldTypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The field type entity 'AggregateId={@event.AggregateId}' could not be found.");

      fieldType.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class FieldTypeTextPropertiesChangedEventHandler : INotificationHandler<FieldType.TextPropertiesChangedEvent>
  {
    private readonly CmsContext _context;

    public FieldTypeTextPropertiesChangedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(FieldType.TextPropertiesChangedEvent @event, CancellationToken cancellationToken)
    {
      FieldTypeEntity fieldType = await _context.FieldTypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The field type entity 'AggregateId={@event.AggregateId}' could not be found.");

      fieldType.SetProperties(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public class FieldTypeUpdatedEventHandler : INotificationHandler<FieldType.UpdatedEvent>
  {
    private readonly CmsContext _context;

    public FieldTypeUpdatedEventHandler(CmsContext context)
    {
      _context = context;
    }

    public async Task Handle(FieldType.UpdatedEvent @event, CancellationToken cancellationToken)
    {
      FieldTypeEntity fieldType = await _context.FieldTypes
        .SingleOrDefaultAsync(x => x.AggregateId == @event.AggregateId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The field type entity 'AggregateId={@event.AggregateId}' could not be found.");

      fieldType.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
