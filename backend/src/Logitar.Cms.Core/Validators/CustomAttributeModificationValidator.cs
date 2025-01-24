using FluentValidation;
using Logitar.Identity.Core;

namespace Logitar.Cms.Core.Validators;

internal class CustomAttributeModificationValidator : AbstractValidator<CustomAttributeModification>
{
  public CustomAttributeModificationValidator()
  {
    RuleFor(x => x.Key).Identifier();
  }
}
