using FluentValidation;

namespace Logitar.Cms.Core.Users.Validators;

internal class ReadOnlyEmailValidator : AbstractValidator<ReadOnlyEmail>
{
  public ReadOnlyEmailValidator()
  {
    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();
  }
}
