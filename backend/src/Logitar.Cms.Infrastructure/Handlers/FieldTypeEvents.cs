﻿using Logitar.Cms.Core.Fields.Events;
using Logitar.Cms.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class FieldTypeEvents : INotificationHandler<FieldTypeBooleanSettingsChanged>,
  INotificationHandler<FieldTypeCreated>,
  INotificationHandler<FieldTypeDateTimeSettingsChanged>,
  INotificationHandler<FieldTypeNumberSettingsChanged>,
  INotificationHandler<FieldTypeRichTextSettingsChanged>,
  INotificationHandler<FieldTypeStringSettingsChanged>,
  INotificationHandler<FieldTypeUniqueNameChanged>,
  INotificationHandler<FieldTypeUpdated>
{
  private readonly CmsContext _context;

  public FieldTypeEvents(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(FieldTypeBooleanSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.SetSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeCreated @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType == null)
    {
      fieldType = new(@event);

      _context.FieldTypes.Add(fieldType);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeDateTimeSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.SetSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeNumberSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.SetSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeRichTextSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.SetSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeStringSettingsChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.SetSettings(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.SetUniqueName(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(FieldTypeUpdated @event, CancellationToken cancellationToken)
  {
    FieldTypeEntity? fieldType = await _context.FieldTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (fieldType != null && fieldType.Version == (@event.Version - 1))
    {
      fieldType.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}