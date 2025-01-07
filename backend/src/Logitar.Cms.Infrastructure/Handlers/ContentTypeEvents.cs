using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Infrastructure.Entities;
using Logitar.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.Infrastructure.Handlers;

internal class ContentTypeEvents : INotificationHandler<ContentTypeCreated>,
  INotificationHandler<ContentTypeFieldDefinitionChanged>,
  INotificationHandler<ContentTypeUniqueNameChanged>,
  INotificationHandler<ContentTypeUpdated>
{
  private readonly ICommandHelper _commandHelper;
  private readonly CmsContext _context;

  public ContentTypeEvents(ICommandHelper commandHelper, CmsContext context)
  {
    _commandHelper = commandHelper;
    _context = context;
  }

  public async Task Handle(ContentTypeCreated @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes.AsNoTracking()
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType == null)
    {
      contentType = new(@event);

      _context.ContentTypes.Add(contentType);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }

  public async Task Handle(ContentTypeFieldDefinitionChanged @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes
      .Include(x => x.Fields)
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType != null && contentType.Version == (@event.Version - 1))
    {
      FieldTypeEntity fieldType = await _context.FieldTypes
        .SingleOrDefaultAsync(x => x.StreamId == @event.FieldDefinition.FieldTypeId.Value, cancellationToken)
        ?? throw new InvalidOperationException($"The field type entity 'StreamId={@event.FieldDefinition.FieldTypeId}' could not be found.");

      contentType.SetField(fieldType, @event);

      await _context.SaveChangesAsync(cancellationToken);

      FieldDefinitionEntity fieldDefinition = contentType.Fields.Single(f => f.Id == @event.FieldDefinition.Id);

      ICommand command = _commandHelper.Update()
        .Set(new Update(CmsDb.FieldIndex.FieldDefinitionName, fieldDefinition.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.FieldIndex.FieldDefinitionId, Operators.IsEqualTo(fieldDefinition.FieldDefinitionId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.UniqueIndex.FieldDefinitionName, fieldDefinition.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.UniqueIndex.FieldDefinitionId, Operators.IsEqualTo(fieldDefinition.FieldDefinitionId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);
    }
  }

  public async Task Handle(ContentTypeUniqueNameChanged @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType != null && contentType.Version == (@event.Version - 1))
    {
      contentType.SetUniqueName(@event);

      await _context.SaveChangesAsync(cancellationToken);

      ICommand command = _commandHelper.Update()
        .Set(new Update(CmsDb.FieldIndex.ContentTypeName, contentType.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.FieldIndex.ContentTypeId, Operators.IsEqualTo(contentType.ContentTypeId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);

      command = _commandHelper.Update()
        .Set(new Update(CmsDb.UniqueIndex.ContentTypeName, contentType.UniqueNameNormalized))
        .Where(new OperatorCondition(CmsDb.UniqueIndex.ContentTypeId, Operators.IsEqualTo(contentType.ContentTypeId)))
        .Build();
      await _context.Database.ExecuteSqlRawAsync(command.Text, command.Parameters.ToArray(), cancellationToken);
    }
  }

  public async Task Handle(ContentTypeUpdated @event, CancellationToken cancellationToken)
  {
    ContentTypeEntity? contentType = await _context.ContentTypes
      .SingleOrDefaultAsync(x => x.StreamId == @event.StreamId.Value, cancellationToken);
    if (contentType != null && contentType.Version == (@event.Version - 1))
    {
      contentType.Update(@event);

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
