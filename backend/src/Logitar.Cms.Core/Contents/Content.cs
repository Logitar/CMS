﻿using Logitar.Cms.Core.Contents.Events;
using Logitar.Cms.Core.Localization;
using Logitar.EventSourcing;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core.Settings;

namespace Logitar.Cms.Core.Contents;

public class Content : AggregateRoot
{
  public static readonly IUniqueNameSettings UniqueNameSettings = new UniqueNameSettings();

  public new ContentId Id => new(base.Id);

  public ContentTypeId ContentTypeId { get; private set; }

  private ContentLocale? _invariant = null;
  private ContentStatus _invariantStatus;
  public ContentLocale Invariant => _invariant ?? throw new InvalidOperationException($"The {nameof(Invariant)} has not been initialized yet.");

  private readonly Dictionary<LanguageId, ContentLocale> _locales = [];
  private readonly Dictionary<LanguageId, ContentStatus> _localeStatuses = [];
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
    _invariantStatus = ContentStatus.Latest;
  }

  public ContentLocale FindLocale(Language language) => FindLocale(language.Id);
  public ContentLocale FindLocale(LanguageId languageId)
  {
    return TryGetLocale(languageId) ?? throw new InvalidOperationException($"The locale 'LanguageId={languageId}' could not be found.");
  }

  public bool HasLocale(Language language) => HasLocale(language.Id);
  public bool HasLocale(LanguageId languageId) => _locales.ContainsKey(languageId);

  public bool IsInvariantPublished() => _invariantStatus == ContentStatus.Published;
  public bool IsLocalePublished(Language language) => IsLocalePublished(language.Id);
  public bool IsLocalePublished(LanguageId languageId) => _localeStatuses.TryGetValue(languageId, out ContentStatus status) && status == ContentStatus.Published;
  public bool IsPublished(LanguageId? languageId) => languageId.HasValue ? IsLocalePublished(languageId.Value) : IsInvariantPublished();

  public void Publish(ActorId? actorId = null)
  {
    PublishInvariant(actorId);

    foreach (LanguageId languageId in _locales.Keys)
    {
      PublishLocale(languageId, actorId);
    }
  }
  public void PublishInvariant(ActorId? actorId = null)
  {
    if (_invariantStatus != ContentStatus.Published)
    {
      Raise(new ContentLocalePublished(LanguageId: null), actorId);
    }
  }
  public bool PublishLocale(Language language, ActorId? actorId = null) => PublishLocale(language.Id, actorId);
  public bool PublishLocale(LanguageId languageId, ActorId? actorId = null)
  {
    if (!HasLocale(languageId))
    {
      return false;
    }
    else if (!IsPublished(languageId))
    {
      Raise(new ContentLocalePublished(languageId), actorId);
    }

    return true;
  }
  protected virtual void Handle(ContentLocalePublished @event)
  {
    if (@event.LanguageId.HasValue)
    {
      _localeStatuses[@event.LanguageId.Value] = ContentStatus.Published;
    }
    else
    {
      _invariantStatus = ContentStatus.Published;
    }
  }

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
      _localeStatuses[@event.LanguageId.Value] = ContentStatus.Latest;
    }
    else
    {
      _invariant = @event.Locale;
      _invariantStatus = ContentStatus.Latest;
    }
  }

  public ContentLocale? TryGetLocale(Language language) => TryGetLocale(language.Id);
  public ContentLocale? TryGetLocale(LanguageId languageId) => _locales.TryGetValue(languageId, out ContentLocale? locale) ? locale : null;

  public void Unpublish(ActorId? actorId = null)
  {
    UnpublishInvariant(actorId);

    foreach (LanguageId languageId in _locales.Keys)
    {
      UnpublishLocale(languageId, actorId);
    }
  }
  public void UnpublishInvariant(ActorId? actorId = null)
  {
    if (_invariantStatus == ContentStatus.Published)
    {
      Raise(new ContentLocaleUnpublished(LanguageId: null), actorId);
    }
  }
  public bool UnpublishLocale(Language language, ActorId? actorId = null) => UnpublishLocale(language.Id, actorId);
  public bool UnpublishLocale(LanguageId languageId, ActorId? actorId = null)
  {
    if (!HasLocale(languageId))
    {
      return false;
    }
    else if (IsPublished(languageId))
    {
      Raise(new ContentLocaleUnpublished(languageId), actorId);
    }

    return true;
  }
  protected virtual void Handle(ContentLocaleUnpublished @event)
  {
    if (@event.LanguageId.HasValue)
    {
      _localeStatuses[@event.LanguageId.Value] = ContentStatus.Latest;
    }
    else
    {
      _invariantStatus = ContentStatus.Latest;
    }
  }

  public override string ToString() => $"{Invariant.DisplayName?.Value ?? Invariant.UniqueName.Value} | {base.ToString()}";
}
