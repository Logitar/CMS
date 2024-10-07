using Logitar.Cms.EntityFrameworkCore.Entities;
using Logitar.Cms.EntityFrameworkCore.Indexing;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Cms.EntityFrameworkCore.Handlers.Indexing;

internal class UpdateUniqueFieldIndexCommandHandler : INotificationHandler<UpdateFieldIndicesCommand>
{
  private readonly CmsContext _context;

  public UpdateUniqueFieldIndexCommandHandler(CmsContext context)
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

    Dictionary<Guid, UniqueFieldIndexEntity> entities = (await _context.UniqueFieldIndex
      .Where(x => x.ContentLocaleId == locale.ContentLocaleId)
      .ToArrayAsync(cancellationToken)
    ).ToDictionary(x => x.FieldDefinitionUid, x => x);

    foreach (UniqueFieldIndexEntity entity in entities.Values)
    {
      if (!fieldValues.ContainsKey(entity.FieldDefinitionUid))
      {
        _context.UniqueFieldIndex.Remove(entity);
      }
    }

    foreach (KeyValuePair<Guid, string> fieldValue in fieldValues)
    {
      if (entities.TryGetValue(fieldValue.Key, out UniqueFieldIndexEntity? entity))
      {
        entity.Value = fieldValue.Value;
      }
      else if (fieldDefinitionsById.TryGetValue(fieldValue.Key, out FieldDefinitionEntity? fieldDefinition) && fieldDefinition.IsUnique)
      {
        entity = new(locale, fieldDefinition, fieldValue.Value);
        entities[fieldValue.Key] = entity;

        _context.UniqueFieldIndex.Add(entity);
      }
    }

    await _context.SaveChangesAsync(cancellationToken);
  }
}
