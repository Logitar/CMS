using Logitar.Cms.Core.Languages;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;

namespace Logitar.Cms.EntityFrameworkCore.Entities;

internal class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }

  public Guid Id { get; private set; }

  public bool IsDefault { get; private set; }
  public string Locale { get; private set; } = string.Empty;
  public string LocaleNormalized
  {
    get => CmsDb.Helper.Normalize(Locale);
    private set { }
  }

  public string DisplayName { get; private set; } = string.Empty;
  public string EnglishName { get; private set; } = string.Empty;
  public string NativeName { get; private set; } = string.Empty;

  public LanguageEntity(Language.CreatedEvent @event) : base(@event)
  {
    Id = @event.AggregateId.ToGuid();

    IsDefault = @event.IsDefault;
    SetLocale(@event.Locale);
  }

  private LanguageEntity() : base()
  {
  }

  public void SetDefault(Language.SetDefaultEvent @event)
  {
    Update(@event);

    IsDefault = @event.IsDefault;
  }

  public void Update(Language.UpdatedEvent @event)
  {
    base.Update(@event);

    if (@event.Locale != null)
    {
      SetLocale(@event.Locale);
    }
  }

  private void SetLocale(Locale locale)
  {
    Locale = locale.Code;

    DisplayName = locale.DisplayName;
    EnglishName = locale.EnglishName;
    NativeName = locale.NativeName;
  }
}
