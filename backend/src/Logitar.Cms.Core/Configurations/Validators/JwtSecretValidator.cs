using FluentValidation;

namespace Logitar.Cms.Core.Configurations.Validators;

public class JwtSecretValidator : AbstractValidator<string>
{
  public JwtSecretValidator()
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(JwtSecretUnit.MaximumLength)
      .MinimumLength(JwtSecretUnit.MinimumLength);
  }
}
