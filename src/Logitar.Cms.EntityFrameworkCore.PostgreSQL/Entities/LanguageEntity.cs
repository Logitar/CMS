using Logitar.Cms.Core.Languages.Events;

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Entities;

internal class LanguageEntity : AggregateEntity
{
  public LanguageEntity(LanguageCreated e) : base(e)
  {
    Locale = e.Locale.Name;
  }

  private LanguageEntity() : base()
  {
  }

  public int LanguageId { get; private set; }

  public string Locale { get; private set; } = string.Empty;
  public bool IsDefault { get; private set; }

  public void SetDefault(SetDefaultLanguage e)
  {
    Update(e);

    IsDefault = e.IsDefault;
  }
}
