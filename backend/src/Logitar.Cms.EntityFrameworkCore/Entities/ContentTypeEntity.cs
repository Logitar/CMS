using Logitar.Cms.Core.ContentTypes.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentTypeEntity : AggregateEntity
{
  public int ContentTypeId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public ContentTypeEntity(ContentTypeCreatedEvent @event) : base(@event)
  {
    UniqueId = @event.AggregateId.ToGuid();

    IsInvariant = @event.IsInvariant;

    UniqueName = @event.UniqueName.Value;
  }

  private ContentTypeEntity() : base()
  {
  }

  public void Update(ContentTypeUpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.DisplayName != null)
    {
      DisplayName = @event.DisplayName.Value?.Value;
    }
    if (@event.Description != null)
    {
      Description = @event.Description.Value?.Value;
    }
  }
}
