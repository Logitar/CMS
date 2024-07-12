using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentItemEntity : AggregateEntity
{
  public int ContentItemId { get; private set; }
  public Guid UniqueId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public List<ContentLocaleEntity> Locales { get; private set; } = [];

  public ContentItemEntity(ContentTypeEntity contentType, ContentCreatedEvent @event) : base(@event)
  {
    UniqueId = @event.AggregateId.ToGuid();

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    Locales.Add(new ContentLocaleEntity(this, language: null, @event.Invariant, @event));
  }

  private ContentItemEntity() : base()
  {
  }

  public override IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = base.GetActorIds().ToList();

    if (ContentType != null)
    {
      actorIds.AddRange(ContentType.GetActorIds());
    }

    foreach (ContentLocaleEntity locale in Locales)
    {
      actorIds.AddRange(locale.GetActorIds());
    }

    return actorIds.AsReadOnly();
  }

  public void SetLocale(LanguageEntity? language, ContentLocaleChangedEvent @event)
  {
    Update(@event);

    ContentLocaleEntity? locale = Locales.SingleOrDefault(l => l.LanguageId == language?.LanguageId);
    if (locale == null)
    {
      locale = new(this, language, @event.Locale, @event);
      Locales.Add(locale);
    }
    else
    {
      locale.Update(@event.Locale, @event);
    }
  }
}
