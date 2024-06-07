using Logitar.Cms.Core.Localization.Events;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Localization;

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

  public override string ToString() => $"{Locale.Culture.DisplayName} | {base.ToString()}";
}
