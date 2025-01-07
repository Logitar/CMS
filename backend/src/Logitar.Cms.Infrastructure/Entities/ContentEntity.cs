using Logitar.Cms.Core.Contents.Events;
using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.Infrastructure.Entities;

public class ContentEntity : AggregateEntity
{
  public int ContentId { get; private set; }
  public Guid Id { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }

  public List<ContentLocaleEntity> Locales { get; private set; } = [];
  public List<UniqueIndexEntity> UniqueIndex { get; private set; } = [];

  public ContentEntity(ContentTypeEntity contentType, ContentCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();

    ContentType = contentType;
    ContentTypeId = contentType.ContentTypeId;

    ContentLocaleEntity invariant = new(this, @event);
    Locales.Add(invariant);
  }

  private ContentEntity() : base()
  {
  }

  public override IReadOnlyCollection<ActorId> GetActorIds() => GetActorIds(includeLocales: true);
  public IReadOnlyCollection<ActorId> GetActorIds(bool includeLocales)
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
        actorIds.AddRange(locale.GetActorIds(includeContent: false));
      }
    }
    return actorIds.AsReadOnly();
  }

  public void SetLocale(LanguageEntity? language, ContentLocaleChanged @event)
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
      locale.Update(@event.Locale, @event);
    }
  }
}
