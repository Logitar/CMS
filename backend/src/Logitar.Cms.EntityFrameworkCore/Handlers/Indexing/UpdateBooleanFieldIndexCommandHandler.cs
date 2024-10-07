using Logitar.Cms.Contracts.FieldTypes;
using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Cms.EntityFrameworkCore.Indexing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Indexing;

internal class UpdateBooleanFieldIndexCommandHandler : INotificationHandler<UpdateFieldIndicesCommand>
{
  private readonly CmsContext _context;

  public UpdateBooleanFieldIndexCommandHandler(CmsContext context)
  {
    _context = context;
  }

  public async Task Handle(UpdateFieldIndicesCommand command, CancellationToken cancellationToken)
  {
    ContentLocaleEntity locale = command.Locale;
    IReadOnlyDictionary<Guid, string> fieldValues = command.FieldValues;

    if (locale.Item == null)
    {
      throw new ArgumentException("The locale item is required.", nameof(command));
    }
    else if (locale.Item.ContentType == null)
    {
      throw new ArgumentException("The locale item content type is required.", nameof(command));
    }
    Dictionary<Guid, FieldDefinitionEntity> fieldDefinitionsById = locale.Item.ContentType.FieldDefinitions
      .ToDictionary(f => f.UniqueId, f => f);

    Dictionary<Guid, BooleanFieldIndexEntity> entities = (await _context.BooleanFieldIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId)
      .ToArrayAsync(cancellationToken)
    ).ToDictionary(x => x.FieldDefinitionUid, x => x);

    foreach (BooleanFieldIndexEntity entity in entities.Values)
    {
      if (!fieldValues.ContainsKey(entity.FieldDefinitionUid))
      {
        _context.BooleanFieldIndex.Remove(entity);
      }
    }

    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      if (bool.TryParse(fieldValue.Value, out bool value))
      {
        if (entities.TryGetValue(fieldValue.Key, out BooleanFieldIndexEntity? entity))
        {
          entity.Value = value;
        }
        else if (fieldDefinitionsById.TryGetValue(fieldValue.Key, out FieldDefinitionEntity? fieldDefinition)
          && fieldDefinition.FieldType?.DataType == DataType.Boolean && fieldDefinition.IsIndexed)
        {
          entity = new(locale, fieldDefinition, value);
          entities[fieldValue.Key] = entity;

          _context.BooleanFieldIndex.Add(entity);
        }
      }
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
