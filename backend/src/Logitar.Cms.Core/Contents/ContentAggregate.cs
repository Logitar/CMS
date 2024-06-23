using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Core.Contents;

public class ContentAggregate : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new ReadOnlyUniqueNameSettings("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_");

  public new ContentId Id => new(base.Id);

  public ContentTypeId ContentTypeId { get; private set; }

  private ContentLocaleUnit? _invariant = null;
  public ContentLocaleUnit Invariant => _invariant ?? throw new InvalidOperationException($"The {nameof(Invariant)} has not been initialized yet.");
  private readonly Dictionary<LanguageId, ContentLocaleUnit> _locales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocaleUnit> Locales => _locales.AsReadOnly();
  public bool HasLocale(LanguageAggregate language) => HasLocale(language.Id);
  public bool HasLocale(LanguageId id) => _locales.ContainsKey(id);
  public ContentLocaleUnit? TryGetLocale(LanguageAggregate language) => TryGetLocale(language.Id);
  public ContentLocaleUnit? TryGetLocale(LanguageId id) => _locales.TryGetValue(id, out ContentLocaleUnit? locale) ? locale : null;
  public ContentLocaleUnit GetLocale(LanguageAggregate language) => GetLocale(language.Id);
  public ContentLocaleUnit GetLocale(LanguageId id) => TryGetLocale(id) ?? throw new InvalidOperationException($"The content locale 'LanguageId={id}' could not be found.");

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
    ContentTypeId = @event.ContentTypeId;

    _invariant = @event.Invariant;
  }

  public void SetInvariant(ContentLocaleUnit invariant, ActorId actorId = default)
  {
    if (!Invariant.Equals(invariant))
    {
      Raise(new ContentLocaleChangedEvent(languageId: null, invariant), actorId);
    }
  }
  public void SetLocale(LanguageAggregate language, ContentLocaleUnit locale, ActorId actorId = default)
  {
    if (!HasLocale(language) || GetLocale(language) != locale)
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
