using Logitar.Cms.Core.Contents;
using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentLocaleEntity
{
  public int ContentLocaleId { get; private set; }

  public int ContentTypeId { get; private set; }
  public ContentItemEntity? ContentItem { get; private set; }
  public int ContentItemId { get; private set; }

  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public string CreatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime UpdatedOn { get; private set; }

  public ContentLocaleEntity(ContentItemEntity contentItem, ContentCreatedEvent @event) : this(contentItem, @event.Invariant, @event)
  {
  }
  public ContentLocaleEntity(ContentItemEntity contentItem, LanguageEntity? language, ContentLocaleChangedEvent @event) : this(contentItem, @event.Locale, @event)
  {
    Language = language;
    LanguageId = language?.LanguageId;
  }
  private ContentLocaleEntity(ContentItemEntity contentItem, ContentLocaleUnit locale, DomainEvent @event)
  {
    ContentTypeId = contentItem.ContentTypeId;
    ContentItem = contentItem;
    ContentItemId = contentItem.ContentItemId;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(locale, @event);
  }

  private ContentLocaleEntity()
  {
  }

  public IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = [new ActorId(CreatedBy), new ActorId(UpdatedBy)];
    if (Language != null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }
    return actorIds;
  }

  public void Update(ContentLocaleChangedEvent @event) => Update(@event.Locale, @event);
  private void Update(ContentLocaleUnit locale, DomainEvent @event)
  {
    UniqueName = locale.UniqueName.Value;
    DisplayName = locale.DisplayName?.Value;
    Description = locale.Description?.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
}
