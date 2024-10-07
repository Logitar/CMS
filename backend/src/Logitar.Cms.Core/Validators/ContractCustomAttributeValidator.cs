using FluentValidation;
using Logitar.Cms.Contracts;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Validators;

public class ContractCustomAttributeValidator : AbstractValidator<CustomAttribute>
{
  public ContractCustomAttributeValidator()
  {
    RuleFor(x => x.Key).SetValidator(new CustomAttributeKeyValidator());
    RuleFor(x => x.Value).SetValidator(new CustomAttributeValueValidator());
  }
}
