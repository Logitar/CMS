using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Contracts.Configurations;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; }

  public UniqueNameSettings() : this(allowedCharacters: null)
  {
  }

  public UniqueNameSettings(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  public UniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
  }
}
