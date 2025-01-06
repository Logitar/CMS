using FluentValidation;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Validators;

internal class CustomAttributeValidator : AbstractValidator<CustomAttribute>
{
  public CustomAttributeValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
