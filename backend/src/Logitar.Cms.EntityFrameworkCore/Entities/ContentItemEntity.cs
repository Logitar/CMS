using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentItemEntity : AggregateEntity
{
  public int ContentItemId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public List<ContentLocaleEntity> Locales { get; private set; } = [];

  public ContentItemEntity(ContentTypeEntity contentType, ContentCreatedEvent @event) : base(@event)
  {
    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    Locales.Add(new ContentLocaleEntity(this, language: null, @event));
  }

  private ContentItemEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds() => GetActorIds(includeLocales: true);
  public IEnumerable<ActorId> GetActorIds(bool includeLocales)
  {
    List<ActorId> actorIds = [.. base.GetActorIds()];

    if (ContentType != null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }

    if (includeLocales)
    {
      foreach (ContentLocaleEntity locale in Locales)
      {
        actorIds.AddRange(locale.GetActorIds(includeItem: false));
      }
    }

    return actorIds.AsReadOnly();
  }

  public void SetLocale(LanguageEntity? language, ContentLocaleChangedEvent @event)
  {
    Update(@event);

    ContentLocaleEntity? locale = Locales.SingleOrDefault(x => x.LanguageId == language?.LanguageId);
    if (locale == null)
    {
      locale = new(this, language, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event);
    }
  }
}
