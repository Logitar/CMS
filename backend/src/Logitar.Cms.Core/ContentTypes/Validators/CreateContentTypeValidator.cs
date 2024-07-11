using FluentValidation;
using Logitar.Cms.Contracts.ContentTypes;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Cms.Core.ContentTypes.Validators;

public class CreateContentTypeValidator : AbstractValidator<CreateContentTypePayload>
{
  public CreateContentTypeValidator()
  {
    RuleFor(x => x.UniqueName).SetValidator(new IdentifierValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
