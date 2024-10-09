using Logitar.Cms.Core.Configurations;
using Logitar.Cms.Core.ContentTypes;
using Logitar.Cms.Core.Languages;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using MediatR;
using ContentType = Logitar.Cms.Core.ContentTypes.ContentType;

namespace Logitar.Cms.Core.Contents;

public class Content : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_");

  public new ContentId Id => new(base.Id);

  public ContentTypeId ContentTypeId { get; private set; }

  private ContentLocale? _invariant = null;
  public ContentLocale Invariant => _invariant ?? throw new InvalidOperationException($"The {nameof(Invariant)} has not been initialized yet.");
  private readonly Dictionary<LanguageId, ContentLocale> _locales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocale> Locales => _locales.AsReadOnly();
  public bool HasLocale(Language language) => HasLocale(language.Id);
  public bool HasLocale(LanguageId languageId) => _locales.ContainsKey(languageId);
  public ContentLocale GetLocale(Language language) => TryGetLocale(language)
    ?? throw new InvalidOperationException($"The content '{this}' has no locale for language '{language}'.");
  public ContentLocale GetLocale(LanguageId languageId) => TryGetLocale(languageId)
    ?? throw new InvalidOperationException($"The content '{this}' has no locale for language 'Id={languageId}'.");
  public ContentLocale? TryGetLocale(Language language) => TryGetLocale(language.Id);
  public ContentLocale? TryGetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocale? locale) ? locale : null;

  public Content() : base()
  {
  }

  public Content(ContentType contentType, ContentLocale invariant, ActorId actorId = default, ContentId? id = null)
    : base((id ?? ContentId.NewId()).AggregateId)
  {
    Raise(new CreatedEvent(contentType.Id, invariant), actorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    ContentTypeId = @event.ContentTypeId;

    _invariant = @event.Invariant;
  }

  public void SetInvariant(ContentLocale invariant, ActorId actorId = default)
  {
    if (_invariant != invariant)
    {
      Raise(new LocaleChangedEvent(languageId: null, invariant), actorId);
    }
  }
  public void SetLocale(Language language, ContentLocale locale, ActorId actorId = default)
  {
    if (!_locales.TryGetValue(language.Id, out ContentLocale? existingLocale) || existingLocale != locale)
    {
      Raise(new LocaleChangedEvent(language.Id, locale), actorId);
    }
  }
  protected virtual void Apply(LocaleChangedEvent @event)
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

  public override string ToString() => $"{Invariant.UniqueName} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public ContentTypeId ContentTypeId { get; }

    public ContentLocale Invariant { get; }

    public CreatedEvent(ContentTypeId contentTypeId, ContentLocale invariant)
    {
      ContentTypeId = contentTypeId;

      Invariant = invariant;
    }
  }

  public class LocaleChangedEvent : DomainEvent, INotification
  {
    public LanguageId? LanguageId { get; }
    public ContentLocale Locale { get; }

    public LocaleChangedEvent(LanguageId? languageId, ContentLocale locale)
    {
      LanguageId = languageId;
      Locale = locale;
    }
  }
}
