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
    get => CmsDb.Normalize(UniqueName);
    private set { }
  }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }
  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public ContentLocaleEntity(ContentItemEntity contentItem, LanguageEntity? language, DomainEvent @event)
  {
    ContentTypeId = contentItem.ContentTypeId;
    ContentItem = contentItem;
    ContentItemId = contentItem.ContentItemId;

    Language = language;
    LanguageId = language?.LanguageId;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    Update(@event);
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

    if (includeItem && ContentItem != null)
    {
      actorIds.AddRange(ContentItem.GetActorIds(includeLocales: false));
    }

    return actorIds.AsReadOnly();
  }

  public void Update(DomainEvent @event)
  {
    if (@event is ContentCreatedEvent created)
    {
      Update(created.Invariant);
    }
    else if (@event is ContentLocaleChangedEvent localeChanged)
    {
      Update(localeChanged.Locale);
    }
    else
    {
      throw new ArgumentException($"The event type '{@event.GetType()}' is not supported.", nameof(@event));
    }

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }
  private void Update(ContentLocaleUnit locale)
  {
    UniqueName = locale.UniqueName.Value;
  }
}
