using Logitar.Cms.Core.Localization.Events;
using Logitar.EventSourcing;

namespace Logitar.Cms.Core.Localization;

public class Language : AggregateRoot
{
  private LanguageUpdated _updated = new();

  public new LanguageId Id => new(base.Id);

  public bool IsDefault { get; private set; }

  private Locale? _locale = null;
  public Locale Locale
  {
    get => _locale ?? throw new InvalidOperationException($"The {nameof(Locale)} has not been initialized yet.");
    set
    {
      if (_locale != value)
      {
        _locale = value;
        _updated.Locale = value;
      }
    }
  }

  public Language() : base()
  {
  }

  public Language(Locale locale, bool isDefault = false, ActorId? actorId = null, LanguageId? id = null) : base((id ?? LanguageId.NewId()).StreamId)
  {
    Raise(new LanguageCreated(locale, isDefault), actorId);
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

  public void Update(ActorId? actorId = null)
  {
    if (_updated.HasChanges)
    {
      Raise(_updated, actorId, DateTime.Now);
      _updated = new();
    }
  }
  protected virtual void Handle(LanguageUpdated @event)
  {
    if (@event.Locale != null)
    {
      _locale = @event.Locale;
    }
  }

  public override string ToString() => $"{Locale} | {base.ToString()}";
}
