using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Cms.Core.Languages;

public class Language : AggregateRoot
{
  private UpdatedEvent _updatedEvent = new();

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
        _updatedEvent.Locale = value;
      }
    }
  }

  public Language(Locale locale, bool isDefault = false, ActorId actorId = default, LanguageId? id = null)
    : base((id ?? LanguageId.NewId()).AggregateId)
  {
    Raise(new CreatedEvent(isDefault, locale), actorId);
  }
  protected virtual void Apply(CreatedEvent @event)
  {
    IsDefault = @event.IsDefault;
    _locale = @event.Locale;
  }

  public void SetDefault(ActorId actorId = default) => SetDefault(isDefault: true, actorId);
  public void SetDefault(bool isDefault, ActorId actorId = default)
  {
    if (IsDefault != isDefault)
    {
      Raise(new SetDefaultEvent(isDefault), actorId);
    }
  }
  protected virtual void Apply(SetDefaultEvent @event)
  {
    IsDefault = @event.IsDefault;
  }

  public void Update(ActorId actorId = default)
  {
    if (_updatedEvent.HasChanges)
    {
      Raise(_updatedEvent, actorId, DateTime.Now);
      _updatedEvent = new();
    }
  }
  protected virtual void Apply(UpdatedEvent @event)
  {
    if (@event.Locale != null)
    {
      _locale = @event.Locale;
    }
  }

  public override string ToString() => $"{Locale} | {base.ToString()}";

  public class CreatedEvent : DomainEvent, INotification
  {
    public bool IsDefault { get; }
    public Locale Locale { get; }

    public CreatedEvent(bool isDefault, Locale locale)
    {
      IsDefault = isDefault;
      Locale = locale;
    }
  }

  public class SetDefaultEvent : DomainEvent, INotification
  {
    public bool IsDefault { get; }

    public SetDefaultEvent(bool isDefault)
    {
      IsDefault = isDefault;
    }
  }

  public class UpdatedEvent : DomainEvent, INotification
  {
    public Locale? Locale { get; set; }

    public bool HasChanges => Locale != null;
  }
}
