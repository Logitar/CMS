using Logitar.Cms.Core;
using Logitar.Cms.Core.Contents;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentLocaleEntity
{
  public int ContentLocaleId { get; private set; }
  public Guid UniqueId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public ContentItemEntity? Item { get; private set; }
  public int ContentItemId { get; private set; }

  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Normalize(UniqueName);
    private set { }
  }

  public Dictionary<Guid, string> Fields { get; private set; } = [];
  public string? FieldsSerialized
  {
    get => Fields.Count == 0 ? null : JsonSerializer.Serialize(Fields);
    private set
    {
      Fields.Clear();

      if (value != null)
      {
        Dictionary<Guid, string>? fields = JsonSerializer.Deserialize<Dictionary<Guid, string>>(value);
        if (fields != null)
        {
          foreach (KeyValuePair<Guid, string> field in fields)
          {
            Fields[field.Key] = field.Value;
          }
        }
      }
    }
  }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public ContentLocaleEntity(ContentItemEntity contentItem, LanguageEntity? language, ContentLocaleUnit locale, DomainEvent @event)
  {
    UniqueId = Guid.NewGuid();

    ContentType = contentItem.ContentType;
    ContentTypeId = contentItem.ContentTypeId;

    Item = contentItem;
    ContentItemId = contentItem.ContentItemId;

    Language = language;
    LanguageId = language?.LanguageId;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();

    Update(locale, @event);
  }

  private ContentLocaleEntity()
  {
  }

  public IEnumerable<ActorId> GetActorIds(bool includeItem)
  {
    List<ActorId> actorIds = [new(CreatedBy), new(UpdatedBy)];

    if (Language != null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }

    if (includeItem && Item != null)
    {
      actorIds.AddRange(Item.GetActorIds(includeLocales: false));
    }

    return actorIds.AsReadOnly();
  }

  public void Update(ContentLocaleUnit locale, DomainEvent @event)
  {
    UniqueName = locale.UniqueName.Value;

    Fields.Clear();
    foreach (KeyValuePair<Guid, string> field in locale.FieldValues)
    {
      Fields[field.Key] = field.Value;
    }

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }
}
