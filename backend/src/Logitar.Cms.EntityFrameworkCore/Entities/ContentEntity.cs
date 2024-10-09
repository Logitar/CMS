using Logitar.Cms.Core.Contents;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentEntity : AggregateEntity
{
  public int ContentId { get; private set; }

  public Guid Id { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public List<ContentLocaleEntity> Locales { get; private set; } = [];

  public ContentEntity(ContentTypeEntity contentType, Content.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    Locales.Add(new ContentLocaleEntity(this, @event));
  }

  private ContentEntity() : base()
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

  public void SetLocale(LanguageEntity? language, Content.LocaleChangedEvent @event)
  {
    ContentLocaleEntity? locale = Locales.SingleOrDefault(l => l.LanguageId == language?.LanguageId);
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
