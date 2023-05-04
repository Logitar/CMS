using FluentValidation;

namespace Logitar.Cms.Core.Configurations.Validators;

internal class ReadOnlyPasswordSettingsValidator : AbstractValidator<ReadOnlyPasswordSettings>
{
  public ReadOnlyPasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(1);

    RuleFor(x => x.RequiredUniqueChars).GreaterThanOrEqualTo(1)
      .LessThanOrEqualTo(x => x.RequiredLength);
  }
}
