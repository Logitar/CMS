using Logitar.Cms.Contracts.Configurations;
using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Core.Contents;

public class ContentAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings();

  public new ContentId Id => new(base.Id);

  private ContentTypeId? _contentTypeId = null;
  public ContentTypeId ContentTypeId => _contentTypeId ?? throw new InvalidOperationException($"The {nameof(ContentTypeId)} has not been initialized yet.");

  private ContentLocaleUnit? _invariant = null;
  public ContentLocaleUnit Invariant => _invariant ?? throw new InvalidOperationException($"The {nameof(Invariant)} has not been initialized yet.");
  private readonly Dictionary<LanguageId, ContentLocaleUnit> _locales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocaleUnit> Locales => _locales.AsReadOnly();
  public ContentLocaleUnit? GetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocaleUnit? locale) ? locale : null;

  public ContentAggregate(AggregateId id) : base(id)
  {
  }

  public ContentAggregate(ContentTypeAggregate contentType, ContentLocaleUnit invariant, ActorId actorId = default, ContentId? id = null)
    : base((id ?? ContentId.NewId()).AggregateId)
  {
    Raise(new ContentCreatedEvent(contentType.Id, invariant), actorId);
  }
  protected virtual void Apply(ContentCreatedEvent @event)
  {
    _contentTypeId = @event.ContentTypeId;

    _invariant = @event.Invariant;
  }

  public void SetLocale(LanguageAggregate language, ContentLocaleUnit locale, ActorId actorId = default)
  {
    ContentLocaleUnit? existingLocale = GetLocale(language.Id);
    if (existingLocale == null || !existingLocale.Equals(locale))
    {
      Raise(new ContentLocaleChangedEvent(language.Id, locale), actorId);
    }
  }
  protected virtual void Apply(ContentLocaleChangedEvent @event)
  {
    if (@event.LanguageId == null)
    {
      _invariant = @event.Locale;
    }
    else
    {
      _locales[@event.LanguageId] = @event.Locale;
    }
  }

  public override string ToString() => $" | {base.ToString()}";
}
