using FluentValidation;
using Logitar.Cms.Core.Languages.Events;
using Logitar.Cms.Core.Languages.Validators;
using Logitar.EventSourcing;
using System.Globalization;

namespace Logitar.Cms.Core.Languages;

public class LanguageAggregate : AggregateRoot
{
  public LanguageAggregate(AggregateId id) : base(id) { }

  public LanguageAggregate(AggregateId actorId, CultureInfo locale) : base()
  {
    LanguageCreated e = new(locale)
    {
      ActorId = actorId
    };
    new LanguageCreatedValidator().ValidateAndThrow(e);

    ApplyChange(e);
  }
  protected virtual void Apply(LanguageCreated e) => Locale = e.Locale;

  public CultureInfo Locale { get; private set; } = CultureInfo.InvariantCulture;
  public bool IsDefault { get; private set; }

  public void SetDefault(AggregateId actorId, bool isDefault = true)
  {
    if (IsDefault != isDefault)
    {
      ApplyChange(new SetDefaultLanguage
      {
        ActorId = actorId,
        IsDefault = isDefault
      });
    }
  }
  protected virtual void Apply(SetDefaultLanguage e) => IsDefault = e.IsDefault;

  public override string ToString() => $"{Locale.DisplayName} ({Locale.Name}) | {base.ToString()}";
}
