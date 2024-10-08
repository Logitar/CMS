using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Contracts.Configurations;

public record UniqueNameSettingsModel : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; }

  public UniqueNameSettingsModel() : this(allowedCharacters: null)
  {
  }

  public UniqueNameSettingsModel(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  public UniqueNameSettingsModel(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
  }
}
