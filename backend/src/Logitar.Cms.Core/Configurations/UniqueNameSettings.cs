using FluentValidation;
using Logitar.Cms.Core.Configurations.Validators;
using Logitar.Identity.Contracts.Settings;

namespace Logitar.Cms.Core.Configurations;

public record UniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; }

  public UniqueNameSettings() : this(allowedCharacters: null)
  {
  }

  public UniqueNameSettings(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  [JsonConstructor]
  public UniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
    new UniqueNameSettingsValidator().ValidateAndThrow(this);
  }
}
