using Logitar.Cms.Core.Localization.Events;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Identity.EntityFrameworkCore.Relational.IdentityDb;

namespace Logitar.Cms.Infrastructure.Entities;

public class LanguageEntity : AggregateEntity
{
  public int LanguageId { get; private set; }
  public Guid Id { get; private set; }

  public bool IsDefault { get; private set; }

  public int LCID { get; private set; }
  public string Code { get; private set; } = string.Empty;
  public string CodeNormalized
  {
    get => Helper.Normalize(Code);
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

  public void SetLocale(LanguageLocaleChanged @event)
  {
    base.Update(@event);

    SetLocale(@event.Locale);
  }
  private void SetLocale(Locale locale)
  {
    LCID = locale.Culture.LCID;
    Code = locale.Culture.Name;
    DisplayName = locale.Culture.DisplayName;
    EnglishName = locale.Culture.EnglishName;
    NativeName = locale.Culture.NativeName;
  }

  public override string ToString() => $"{DisplayName} ({Code}) | {base.ToString()}";
}
