using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes.Validators;

internal class CreateFieldDefinitionValidator : AbstractValidator<CreateFieldDefinitionPayload>
{
  public CreateFieldDefinitionValidator()
  {
    RuleFor(x => x.ContentTypeId).NotEmpty();

    RuleFor(x => x.FieldTypeId).NotEmpty();

    RuleFor(x => x.UniqueName).SetValidator(new IdentifierValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder), () => RuleFor(x => x.Description!).SetValidator(new PlaceholderValidator()));
  }
}
