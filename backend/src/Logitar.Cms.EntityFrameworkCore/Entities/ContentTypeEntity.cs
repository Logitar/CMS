using Logitar.Cms.Core.ContentTypes;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentTypeEntity : AggregateEntity
{
  public int ContentTypeId { get; private set; }

  public Guid Id { get; private set; }

  public bool IsInvariant { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Helper.Normalize(UniqueName);
    private set { }
  }
  public string? DisplayName { get; private set; }
  public string? Description { get; private set; }

  public List<ContentEntity> Contents { get; private set; } = [];
  public List<ContentLocaleEntity> ContentLocales { get; private set; } = [];

  public ContentTypeEntity(ContentType.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    IsInvariant = @event.IsInvariant;

    UniqueName = @event.UniqueName.Value;
  }

  private ContentTypeEntity() : base()
  {
  }

  public void Update(ContentType.UpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.UniqueName != null)
    {
      UniqueName = @event.UniqueName.Value;
    }
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
