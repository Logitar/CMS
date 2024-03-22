using FluentValidation;
using Logitar.Cms.Contracts.Archetypes;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.Archetypes.Validators;

public class CreateArchetypeValidator : AbstractValidator<CreateArchetypePayload>
{
  public CreateArchetypeValidator()
  {
    RuleFor(x => x.Identifier).SetValidator(new IdentifierValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
