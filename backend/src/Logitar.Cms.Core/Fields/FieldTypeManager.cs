using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Fields.Events;
using Logitar.Cms.Core.Fields.Settings;
using Logitar.EventSourcing;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Fields;

internal class FieldTypeManager : IFieldTypeManager
{
  private readonly IContentTypeQuerier _contentTypeQuerier;
  private readonly IFieldTypeQuerier _fieldTypeQuerier;
  private readonly IFieldTypeRepository _fieldTypeRepository;

  public FieldTypeManager(IContentTypeQuerier contentTypeQuerier, IFieldTypeQuerier fieldTypeQuerier, IFieldTypeRepository fieldTypeRepository)
  {
    _contentTypeQuerier = contentTypeQuerier;
    _fieldTypeQuerier = fieldTypeQuerier;
    _fieldTypeRepository = fieldTypeRepository;
  }

  public async Task SaveAsync(FieldType fieldType, CancellationToken cancellationToken)
  {
    UniqueName? uniqueName = null;
    RelatedContentSettings? relatedContentSettings = null;
    foreach (IEvent change in fieldType.Changes)
    {
      if (change is FieldTypeCreated created)
      {
        uniqueName = created.UniqueName;
      }
      else if (change is FieldTypeUniqueNameChanged uniqueNameChanged)
      {
        uniqueName = uniqueNameChanged.UniqueName;
      }
      else if (change is FieldTypeRelatedContentSettingsChanged relatedContentSettingsChanged)
      {
        relatedContentSettings = relatedContentSettingsChanged.Settings;
      }
    }

    if (uniqueName != null)
    {
      FieldTypeId? conflictId = await _fieldTypeQuerier.FindIdAsync(uniqueName, cancellationToken);
      if (conflictId.HasValue && !conflictId.Value.Equals(fieldType.Id))
      {
        throw new UniqueNameAlreadyUsedException(fieldType, conflictId.Value);
      }
    }

    if (relatedContentSettings != null)
    {
      _ = await _contentTypeQuerier.ReadAsync(relatedContentSettings.ContentTypeId, cancellationToken)
        ?? throw new ContentTypeNotFoundException(relatedContentSettings.ContentTypeId, nameof(relatedContentSettings.ContentTypeId));
    }

    await _fieldTypeRepository.SaveAsync(fieldType, cancellationToken);
  }
}
