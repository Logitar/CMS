using Logitar.Cms.Core.Languages.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Languages;

public class LanguageAggregate : AggregateRoot
{
  public new LanguageId Id => new(base.Id);

  public bool IsDefault { get; private set; }

  private LocaleUnit? _locale = null;
  public LocaleUnit Locale => _locale ?? throw new InvalidOperationException($"The {nameof(Locale)} has not been initialized yet.");

  public LanguageAggregate() : base()
  {
  }

  public LanguageAggregate(LocaleUnit locale, bool isDefault = false, ActorId actorId = default, LanguageId? id = null)
    : base((id ?? LanguageId.NewId()).AggregateId)
  {
    Raise(new LanguageCreatedEvent(isDefault, locale), actorId);
  }
  protected virtual void Apply(LanguageCreatedEvent @event)
  {
    IsDefault = @event.IsDefault;

    _locale = @event.Locale;
  }

  public void Delete(ActorId actorId = default)
  {
    if (!IsDeleted)
    {
      Raise(new LanguageDeletedEvent(), actorId);
    }
  }

  public void SetDefault(bool isDefault, ActorId actorId = default)
  {
    if (IsDefault != isDefault)
    {
      Raise(new LanguageSetDefaultEvent(isDefault), actorId);
    }
  }
  protected virtual void Apply(LanguageSetDefaultEvent @event)
  {
    IsDefault = @event.IsDefault;
  }

  public override string ToString() => $"{Locale.Culture.DisplayName} ({Locale.Code}) | {base.ToString()}";
}
