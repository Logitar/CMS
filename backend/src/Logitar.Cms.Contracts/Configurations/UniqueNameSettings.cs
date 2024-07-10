using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Contracts.Configurations;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; set; }
}
