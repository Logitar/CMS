using FluentValidation;
using Logitar.Cms.Core.Configurations.Validators;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Settings;

namespace Logitar.Cms.Core.Configurations;

public record ReadOnlyUniqueNameSettings : IUniqueNameSettings
{
  public string? AllowedCharacters { get; }

  public ReadOnlyUniqueNameSettings() : this(new UniqueNameSettings())
  {
  }

  public ReadOnlyUniqueNameSettings(IUniqueNameSettings uniqueName) : this(uniqueName.AllowedCharacters)
  {
  }

  [JsonConstructor]
  public ReadOnlyUniqueNameSettings(string? allowedCharacters)
  {
    AllowedCharacters = allowedCharacters;
    new UniqueNameSettingsValidator().ValidateAndThrow(this);
  }
}
