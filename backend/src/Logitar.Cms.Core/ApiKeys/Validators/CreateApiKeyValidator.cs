using FluentValidation;
using Logitar.Cms.Contracts.ApiKeys;
using Logitar.Cms.Core.Validators;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ApiKeys.Validators;

public class CreateApiKeyValidator : AbstractValidator<CreateApiKeyPayload>
{
  public CreateApiKeyValidator()
  {
    RuleFor(x => x.DisplayName).SetValidator(new DisplayNameValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new ContractCustomAttributeValidator());
  }
}
