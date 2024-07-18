using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Core.Contents;

public class ContentAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new ReadOnlyUniqueNameSettings();

  public new ContentId Id => new(base.Id);

  public ContentTypeId? _contentTypeId = null;
  public ContentTypeId ContentTypeId => _contentTypeId ?? throw new InvalidOperationException($"The {nameof(ContentTypeId)} has not been initialized yet.");

  private ContentLocaleUnit? _invariant = null;
  public ContentLocaleUnit Invariant => _invariant ?? throw new InvalidOperationException($"The {nameof(Invariant)} has not been initialized yet.");

  private readonly Dictionary<LanguageId, ContentLocaleUnit> _locales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocaleUnit> Locales => _locales.AsReadOnly();
  public bool HasLocale(LanguageAggregate language) => HasLocale(language.Id);
  public bool HasLocale(LanguageId languageId) => _locales.ContainsKey(languageId);
  public ContentLocaleUnit GetLocale(LanguageAggregate language) => GetLocale(language.Id);
  public ContentLocaleUnit GetLocale(LanguageId languageId) => TryGetLocale(languageId) ?? throw new InvalidOperationException($"The content has no locale for language 'Id={languageId}'.");
  public ContentLocaleUnit? TryGetLocale(LanguageAggregate language) => TryGetLocale(language.Id);
  public ContentLocaleUnit? TryGetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocaleUnit? locale) ? locale : null;

  public ContentAggregate() : base()
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

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new ContentDeletedEvent(), actorId);
    }
  }

  public void SetInvariant(ContentLocaleUnit invariant, ActorId actorId = default)
  {
    if (_invariant != invariant)
    {
      Raise(new ContentLocaleChangedEvent(languageId: null, invariant), actorId);
    }
  }
  public void SetLocale(LanguageAggregate language, ContentLocaleUnit locale, ActorId actorId = default)
  {
    ContentLocaleUnit? existingLocale = TryGetLocale(language);
    if (existingLocale == null || existingLocale != locale)
    {
      Raise(new ContentLocaleChangedEvent(language.Id, locale), actorId);
    }
  }
  protected virtual void Apply(ContentLocaleChangedEvent @event)
  {
    if (@event.LanguageId.HasValue)
    {
      _locales[@event.LanguageId.Value] = @event.Locale;
    }
    else
    {
      _invariant = @event.Locale;
    }
  }

  public override string ToString() => $"{Invariant.UniqueName.Value} | {base.ToString()}";
}
