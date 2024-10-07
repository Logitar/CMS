using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes.Validators;

public class CreateFieldDefinitionValidator : AbstractValidator<CreateFieldDefinitionPayload>
{
  public CreateFieldDefinitionValidator(bool isInvariant)
  {
    RuleFor(x => x.FieldTypeId).NotEmpty();

    if (isInvariant)
    {
      RuleFor(x => x.IsInvariant).Equal(true).WithMessage("'{PropertyName}' must be true when the content type is invariant.");
    }

    RuleFor(x => x.UniqueName).SetValidator(new IdentifierValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder), () => RuleFor(x => x.Placeholder!).SetValidator(new PlaceholderValidator()));
  }
}
