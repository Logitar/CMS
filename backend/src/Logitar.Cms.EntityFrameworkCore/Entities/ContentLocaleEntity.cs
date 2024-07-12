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

  public IEnumerable<ActorId> GetActorIds() => [new(CreatedBy), new(UpdatedBy)];

  public void Update(ContentLocaleUnit locale, DomainEvent @event)
  {
    UniqueName = locale.UniqueName.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }
}
