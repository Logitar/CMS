using FluentValidation;

namespace Logitar.Cms.Core.ContentTypes.Validators;

public class PlaceholderValidator : AbstractValidator<string>
{
  public PlaceholderValidator()
  {
    RuleFor(x => x).NotEmpty().MaximumLength(PlaceholderUnit.MaximumLength);
  }
}
