using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes.Validators;

internal class UpdateFieldDefinitionValidator : AbstractValidator<UpdateFieldDefinitionPayload>
{
  public UpdateFieldDefinitionValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).SetValidator(new IdentifierValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Placeholder?.Value), () => RuleFor(x => x.Placeholder!.Value!).SetValidator(new PlaceholderValidator()));
  }
}
