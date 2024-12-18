using Logitar.Cms.Core.Localization;
using Logitar.Cms.Core.Localization.Events;
using System.Globalization;

namespace Logitar.Cms.Infrastructure.Entities;

internal class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }
  public Guid Id { get; private set; }

  public bool IsDefault { get; private set; }

  public int LCID { get; private set; }
  public string Code { get; private set; } = string.Empty;
  public string CodeNormalized
  {
    get => CmsDb.Helper.Normalize(Code);
    private set { }
  }
  public string DisplayName { get; private set; } = string.Empty;
  public string EnglishName { get; private set; } = string.Empty;
  public string NativeName { get; private set; } = string.Empty;

  public LanguageEntity(LanguageCreated @event) : base(@event)
  {
    Id = @event.StreamId.ToGuid();

    IsDefault = @event.IsDefault;

    SetLocale(@event.Locale);
  }

  private LanguageEntity() : base()
  {
  }

  public void SetDefault(LanguageSetDefault @event)
  {
    base.Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void Update(LanguageUpdated @event)
  {
    base.Update(@event);

    if (@event.Locale != null)
    {
      SetLocale(@event.Locale);
    }
  }

  private void SetLocale(Locale locale)
  {
    CultureInfo culture = locale.Culture;
    LCID = culture.LCID;
    Code = culture.Name;
    DisplayName = culture.DisplayName;
    EnglishName = culture.EnglishName;
    NativeName = culture.NativeName;
  }
}
