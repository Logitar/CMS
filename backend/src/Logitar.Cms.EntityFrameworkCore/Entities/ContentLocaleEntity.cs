using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentLocaleEntity
{
  public int ContentLocaleId { get; private set; }

  public ContentItemEntity? ContentItem { get; private set; }
  public int ContentItemId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => UniqueName.ToUpper();
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public long Version { get; private set; }

  public string CreatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime CreatedOn { get; private set; }

  public string UpdatedBy { get; private set; } = ActorId.DefaultValue;
  public DateTime UpdatedOn { get; private set; }

  public ContentLocaleEntity(ContentItemEntity contentItem, ContentCreatedEvent @event)
  {
    ContentItem = contentItem;
    ContentItemId = contentItem.ContentItemId;

    UniqueName = @event.UniqueName.Value;

    Version = 1;

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.ToUniversalTime();

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.ToUniversalTime();
  }

  private ContentLocaleEntity()
  {
  }
}
