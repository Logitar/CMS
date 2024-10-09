using Logitar.Cms.Core.Contents;
using Logitar.EventSourcing;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class ContentLocaleEntity
{
  public int ContentLocaleId { get; private set; }

  public ContentTypeEntity? ContentType { get; private set; }
  public int ContentTypeId { get; private set; }
  public ContentEntity? Content { get; private set; }
  public int ContentId { get; private set; }
  public LanguageEntity? Language { get; private set; }
  public int? LanguageId { get; private set; }

  public string UniqueName { get; private set; } = string.Empty;
  public string UniqueNameNormalized
  {
    get => CmsDb.Helper.Normalize(UniqueName);
    private set { }
  }

  public string CreatedBy { get; private set; } = string.Empty;
  public DateTime CreatedOn { get; private set; }
  public string UpdatedBy { get; private set; } = string.Empty;
  public DateTime UpdatedOn { get; private set; }

  public ContentLocaleEntity(ContentEntity content, Content.CreatedEvent @event) : this(content)
  {
    Update(@event.Invariant, @event);

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();
  }
  public ContentLocaleEntity(ContentEntity content, LanguageEntity? language, Content.LocaleChangedEvent @event) : this(content, language)
  {
    Update(@event.Locale, @event);

    CreatedBy = @event.ActorId.Value;
    CreatedOn = @event.OccurredOn.AsUniversalTime();
  }

  private ContentLocaleEntity()
  {
  }

  private ContentLocaleEntity(ContentEntity content, LanguageEntity? language = null)
  {
    ContentType = content.ContentType;
    ContentTypeId = content.ContentTypeId;
    Content = content;
    ContentId = content.ContentId;
    Language = language;
    LanguageId = language?.LanguageId;
  }

  public IEnumerable<ActorId> GetActorIds()
  {
    List<ActorId> actorIds = new(capacity: 4)
    {
      new(CreatedBy),
      new(UpdatedBy)
    };
    if (Language != null)
    {
      actorIds.AddRange(Language.GetActorIds());
    }
    return actorIds.AsReadOnly();
  }

  public void Update(Content.LocaleChangedEvent @event)
  {
    Update(@event.Locale, @event);
  }
  private void Update(ContentLocale locale, DomainEvent @event)
  {
    UniqueName = locale.UniqueName.Value;

    UpdatedBy = @event.ActorId.Value;
    UpdatedOn = @event.OccurredOn.AsUniversalTime();
  }

  public override bool Equals(object? obj) => obj is ContentLocaleEntity entity && entity.ContentLocaleId == ContentLocaleId;
  public override int GetHashCode() => ContentLocaleId.GetHashCode();
  public override string ToString() => $"{GetType()} (ContentLocaleId={ContentLocaleId})";
}
