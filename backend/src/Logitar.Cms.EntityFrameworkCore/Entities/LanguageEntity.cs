using Logitar.Cms.Core.Localization.Events;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }

  public bool IsDefault { get; private set; }
  public string Locale { get; private set; } = string.Empty;
  public string LocaleNormalized
  {
    get => Locale.ToUpper();
    private set { }
  }

  public LanguageEntity(LanguageCreatedEvent @event) : base(@event)
  {
    IsDefault = @event.IsDefault;
    Locale = @event.Locale.Code;
  }

  private LanguageEntity()
  {
  }
}
