using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Contents;

public class Content : AggregateRoot
{
  public new ContentId Id => new(base.Id);

  public ContentTypeId ContentTypeId { get; private set; }

  private ContentLocale? _invariant = null;
  public ContentLocale Invariant => _invariant ?? throw new InvalidOperationException($"The {nameof(Invariant)} has not been initialized yet.");

  private readonly Dictionary<LanguageId, ContentLocale> _locales = [];
  public IReadOnlyDictionary<LanguageId, ContentLocale> Locales => _locales.AsReadOnly();

  public Content() : base()
  {
  }

  public Content(ContentType contentType, ContentLocale invariant, ActorId? actorId = null, ContentId? contentId = null) : base(contentId?.StreamId)
  {
    Raise(new ContentCreated(contentType.Id, invariant), actorId);
  }
  protected virtual void Handle(ContentCreated @event)
  {
    ContentTypeId = @event.ContentTypeId;

    _invariant = @event.Invariant;
  }

  public ContentLocale FindLocale(Language language) => FindLocale(language.Id);
  public ContentLocale FindLocale(LanguageId languageId)
  {
    return TryGetLocale(languageId) ?? throw new InvalidOperationException($"The locale 'LanguageId={languageId}' could not be found.");
  }

  public bool HasLocale(Language language) => HasLocale(language.Id);
  public bool HasLocale(LanguageId languageId) => _locales.ContainsKey(languageId);

  public void SetInvariant(ContentLocale invariant, ActorId? actorId = null)
  {
    if (!Invariant.Equals(invariant))
    {
      Raise(new ContentLocaleChanged(LanguageId: null, invariant), actorId);
    }
  }
  public void SetLocale(Language language, ContentLocale locale, ActorId? actorId = null)
  {
    ContentLocale? existingLocale = TryGetLocale(language);
    if (existingLocale == null || !existingLocale.Equals(locale))
    {
      Raise(new ContentLocaleChanged(language.Id, locale), actorId);
    }
  }
  protected virtual void Handle(ContentLocaleChanged @event)
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

  public ContentLocale? TryGetLocale(Language language) => TryGetLocale(language.Id);
  public ContentLocale? TryGetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocale? locale) ? locale : null;

  public override string ToString() => $"{Invariant.DisplayName?.Value ?? Invariant.UniqueName.Value} | {base.ToString()}";
}
