using Logitar.Cms.Core.Contents.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentItemEntity : AggregateEntity
{
  public int ContentItemId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public List<ContentLocaleEntity> ContentLocales { get; private set; } = [];

  public ContentItemEntity(ContentTypeEntity contentType, ContentCreatedEvent @event) : base(@event)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    ContentLocales.Add(new ContentLocaleEntity(this, @event));
  }

  private ContentItemEntity()
  {
  }
}
