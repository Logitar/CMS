using Logitar.Cms.Core.Languages.Events;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }
  public Guid UniqueId { get; private set; }

  public bool IsDefault { get; private set; }

  public int LCID { get; private set; }
  public string Code { get; private set; } = string.Empty;
  public string CodeNormalized
  {
    get => CmsDb.Normalize(Code);
    private set { }
  }

  public string DisplayName { get; private set; } = string.Empty;
  public string EnglishName { get; private set; } = string.Empty;
  public string NativeName { get; private set; } = string.Empty;

  public LanguageEntity(LanguageCreatedEvent @event) : base(@event)
  {
    UniqueId = @event.AggregateId.ToGuid();

    IsDefault = @event.IsDefault;

    SetLocale(@event.Locale);
  }

  private LanguageEntity() : base()
  {
  }

  public void SetDefault(LanguageSetDefaultEvent @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  private void SetLocale(LocaleUnit locale)
  {
    Code = locale.Code;

    SetCulture(locale.Culture);
  }
  private void SetCulture(CultureInfo culture)
  {
    LCID = culture.LCID;
    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }
}
