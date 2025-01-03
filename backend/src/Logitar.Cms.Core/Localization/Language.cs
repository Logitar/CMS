﻿using Logitar.Cms.Core.Localization.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Localization;

public class Language : AggregateRoot
{
  public new LanguageId Id => new(base.Id);

  public bool IsDefault { get; private set; }

  private Locale? _locale = null;
  public Locale Locale => _locale ?? throw new InvalidOperationException($"The {nameof(Locale)} has not been initialized yet.");

  public Language() : base()
  {
  }

  public Language(Locale locale, bool isDefault = false, ActorId? actorId = null, LanguageId? languageId = null) : base(languageId?.StreamId)
  {
    Raise(new LanguageCreated(isDefault, locale), actorId);
  }
  protected virtual void Handle(LanguageCreated @event)
  {
    IsDefault = @event.IsDefault;

    _locale = @event.Locale;
  }

  public void SetDefault(bool isDefault = true, ActorId? actorId = null)
  {
    if (IsDefault != isDefault)
    {
      Raise(new LanguageSetDefault(isDefault), actorId);
    }
  }
  protected virtual void Handle(LanguageSetDefault @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void SetLocale(Locale locale, ActorId? actorId = null)
  {
    if (Locale != locale)
    {
      Raise(new LanguageLocaleChanged(locale), actorId);
    }
  }
  protected virtual void Handle(LanguageLocaleChanged @event)
  {
    _locale = @event.Locale;
  }

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
