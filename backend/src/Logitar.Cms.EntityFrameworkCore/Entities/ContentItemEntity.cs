using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;
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

  public IEnumerable<ActorId> GetActorIds(bool includeLocales)
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();
    if (ContentType != null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }
    if (includeLocales)
    {
      actorIds.AddRange(ContentLocales.SelectMany(locale => locale.GetActorIds()));
    }
    return actorIds;
  }

  public void SetLocale(LanguageEntity? language, ContentLocaleChangedEvent @event)
  {
    Update(@event);

    ContentLocaleEntity? locale = language == null
      ? ContentLocales.SingleOrDefault(l => l.LanguageId == null)
      : ContentLocales.SingleOrDefault(l => l.LanguageId == language.LanguageId);
    if (locale == null)
    {
      locale = new(this, language, @event);
      ContentLocales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }
}
